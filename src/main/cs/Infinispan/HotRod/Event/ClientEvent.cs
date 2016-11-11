﻿#pragma warning disable 1591
namespace Infinispan.HotRod.Event
{
    public enum EventType: byte
    {
        CLIENT_CACHE_ENTRY_CREATED,
        CLIENT_CACHE_ENTRY_MODIFIED,
        CLIENT_CACHE_ENTRY_REMOVED,
        CLIENT_CACHE_ENTRY_EXPIRED,
        CLIENT_CACHE_ENTRY_CUSTOM,
        CLIENT_CACHE_FAILOVER
    };

    public abstract class ClientEvent
    {
        public abstract EventType getType();
    };

    public class ClientCacheEntryCreatedEvent<K> : ClientEvent
    {
        public ClientCacheEntryCreatedEvent(K key, ulong version, bool commandRetried)
        {
            this.key = key;
            this.version = version;
            this.commandRetried = commandRetried;
        }

        public K getKey() { return key; }
        public ulong getVersion() { return version; }
        public bool isCommandRetried() { return commandRetried; }
        override public EventType getType() { return EventType.CLIENT_CACHE_ENTRY_CREATED; }
        private K key;
        private ulong version;
        private bool commandRetried;
    };

    public class ClientCacheEntryModifiedEvent<K> : ClientEvent
    {
        public ClientCacheEntryModifiedEvent(K key, ulong version, bool commandRetried)
        {
            this.key = key;
            this.version = version;
            this.commandRetried = commandRetried;
        }
        public K getKey() { return key; }
        public ulong getVersion() { return version; }
        public bool isCommandRetried() { return commandRetried; }
        override public EventType getType() { return EventType.CLIENT_CACHE_ENTRY_MODIFIED; }
        private K key;
        private ulong version;
        private bool commandRetried;
    };

    public class ClientCacheEntryExpiredEvent<K> : ClientEvent
    {
        public ClientCacheEntryExpiredEvent(K key)
        {
            this.key = key;
        }
        public K getKey() { return key; }
        override public EventType getType() { return EventType.CLIENT_CACHE_ENTRY_EXPIRED; }
        private K key;
    };

    public class ClientCacheEntryRemovedEvent<K> : ClientEvent
    {
        public ClientCacheEntryRemovedEvent(K key, bool commandRetried)
        {
            this.key = key;
            this.commandRetried = commandRetried;
        }
        public K getKey() { return key; }
        public bool isCommandRetried() { return commandRetried; }
        override public EventType getType() { return EventType.CLIENT_CACHE_ENTRY_MODIFIED; }
        private K key;
        private bool commandRetried;
    };

    public class ClientCacheEntryCustomEvent : ClientEvent
    {
        public ClientCacheEntryCustomEvent(char[] data, bool commandRetried)
        {
            this.data = data;
            this.commandRetried = commandRetried;
        }
        char[] getEventData() { return data; }
        bool isCommandRetried() { return commandRetried; }
        override public EventType getType() { return EventType.CLIENT_CACHE_ENTRY_CUSTOM; }
        char[] data;
        bool commandRetried;
    };

    public class ClientCacheFailoverEvent : ClientEvent
    {
        override public EventType getType() { return EventType.CLIENT_CACHE_FAILOVER; }

    };
}



