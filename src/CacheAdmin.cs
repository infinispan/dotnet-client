using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    [Flags]
    public enum AdminFlag
    {
        Permanent = 1,
        Volatile = 2
    }

    public class CacheAdmin
    {
        private readonly InfinispanClient _client;

        internal CacheAdmin(InfinispanClient client)
        {
            _client = client;
        }

        public async Task CreateCache(string name, string configuration = null, AdminFlag flags = 0)
        {
            await ExecCacheTask("@@cache@create", name, configuration, null, flags);
        }

        public async Task CreateCacheWithTemplate(string name, string template, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            if (!string.IsNullOrEmpty(template))
                parameters.Add(("template", Bytes(template)));
            AddFlags(parameters, flags);
            await _client.Exec("@@cache@create", parameters);
        }

        public async Task GetOrCreateCache(string name, string configuration = null, AdminFlag flags = 0)
        {
            await ExecCacheTask("@@cache@getorcreate", name, configuration, null, flags);
        }

        public async Task GetOrCreateCacheWithTemplate(string name, string template, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            if (!string.IsNullOrEmpty(template))
                parameters.Add(("template", Bytes(template)));
            AddFlags(parameters, flags);
            await _client.Exec("@@cache@getorcreate", parameters);
        }

        public async Task RemoveCache(string name, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            AddFlags(parameters, flags);
            await _client.Exec("@@cache@remove", parameters);
        }

        public async Task<ISet<string>> GetCacheNames()
        {
            var result = await _client.Exec("@@cache@names", new List<(string, byte[])>());
            var names = new HashSet<string>();
            if (result != null && result.Length > 0)
            {
                var json = Encoding.UTF8.GetString(result);
                foreach (Match m in Regex.Matches(json, "\"([^\"]+)\""))
                {
                    names.Add(m.Groups[1].Value);
                }
            }
            return names;
        }

        public async Task ReindexCache(string name)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            await _client.Exec("@@cache@reindex", parameters);
        }

        public async Task UpdateIndexSchema(string name)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            await _client.Exec("@@cache@updateindexschema", parameters);
        }

        public async Task UpdateConfigurationAttribute(string name, string attribute, string value, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name)),
                ("attribute", Bytes(attribute)),
                ("value", Bytes(value))
            };
            AddFlags(parameters, flags);
            await _client.Exec("@@cache@updateConfigurationAttribute", parameters);
        }

        public async Task AssignAlias(string cacheName, string aliasName, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(cacheName)),
                ("alias", Bytes(aliasName))
            };
            AddFlags(parameters, flags);
            await _client.Exec("@@cache@assignAlias", parameters);
        }

        public async Task CreateTemplate(string name, string configuration, AdminFlag flags = 0)
        {
            await ExecCacheTask("@@template@create", name, configuration, null, flags);
        }

        public async Task RemoveTemplate(string name, AdminFlag flags = 0)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            AddFlags(parameters, flags);
            await _client.Exec("@@template@remove", parameters);
        }

        public SchemaAdmin Schemas()
        {
            return new SchemaAdmin(_client);
        }

        private async Task ExecCacheTask(string task, string name, string configuration, string template, AdminFlag flags)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            if (!string.IsNullOrEmpty(configuration))
                parameters.Add(("configuration", Bytes(configuration)));
            if (!string.IsNullOrEmpty(template))
                parameters.Add(("template", Bytes(template)));
            AddFlags(parameters, flags);
            await _client.Exec(task, parameters);
        }

        private static void AddFlags(List<(string Name, byte[] Value)> parameters, AdminFlag flags)
        {
            if (flags != 0)
            {
                var parts = new List<string>();
                if ((flags & AdminFlag.Permanent) != 0) parts.Add("PERMANENT");
                if ((flags & AdminFlag.Volatile) != 0) parts.Add("VOLATILE");
                parameters.Add(("flags", Bytes(string.Join(",", parts))));
            }
        }

        private static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);
    }

    public class SchemaAdmin
    {
        private readonly InfinispanClient _client;

        internal SchemaAdmin(InfinispanClient client)
        {
            _client = client;
        }

        public async Task<string> CreateOrUpdate(string name, string content, SchemaOp op = SchemaOp.Save, bool force = false)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name)),
                ("content", Bytes(content)),
                ("op", Bytes(OpCode(op)))
            };
            if (force)
                parameters.Add(("force", Bytes("f")));
            var result = await _client.Exec("@@schemas@createOrUpdate", parameters);
            return result != null ? Encoding.UTF8.GetString(result) : null;
        }

        public async Task Create(string name, string content)
        {
            await CreateOrUpdate(name, content, SchemaOp.Create);
        }

        public async Task Update(string name, string content, bool force = false)
        {
            await CreateOrUpdate(name, content, SchemaOp.Update, force);
        }

        public async Task Save(string name, string content, bool force = false)
        {
            await CreateOrUpdate(name, content, SchemaOp.Save, force);
        }

        public async Task<string> Delete(string name)
        {
            var parameters = new List<(string Name, byte[] Value)>
            {
                ("name", Bytes(name))
            };
            var result = await _client.Exec("@@schemas@delete", parameters);
            return result != null ? Encoding.UTF8.GetString(result) : null;
        }

        private static string OpCode(SchemaOp op) => op switch
        {
            SchemaOp.Create => "c",
            SchemaOp.Update => "u",
            SchemaOp.Save => "s",
            _ => "s"
        };

        private static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);
    }

    public enum SchemaOp
    {
        Create,
        Update,
        Save
    }
}
