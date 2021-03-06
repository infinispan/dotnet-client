%typemap(csinterfaces) infinispan::hotrod::ConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ConfigurationBuilder %{

    private SWIGGen.FailOverRequestBalancingStrategyProducerDelegate id;

    public Infinispan.HotRod.SWIG.Configuration Create() {
        return create();
    }

    public Infinispan.HotRod.SWIG.ServerConfigurationBuilder AddServer() {
        return addServer();
    }

    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder ConnectionPool() {
        return connectionPool();
    }

    public Infinispan.HotRod.SWIG.SslConfigurationBuilder Ssl() {
        return ssl();
    }

    public Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder NearCache() {
        return nearCache();
    }

    public Infinispan.HotRod.SWIG.SecurityConfigurationBuilder Security() {
        return security();
    }

    public Infinispan.HotRod.SWIG.ConfigurationBuilder AddServers(string _serverList) {
        return addServers(_serverList);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder ConnectionTimeout(int _connectionTimeout) {
        return connectionTimeout(_connectionTimeout);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder ForceReturnValues(bool _forceReturnValues) {
        return forceReturnValues(_forceReturnValues);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder KeySizeEstimate(int _keySizeEstimate) {
        return keySizeEstimate(_keySizeEstimate);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder ProtocolVersion(string _protocolVersion) {
        return protocolVersion(_protocolVersion);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder SocketTimeout(int _socketTimeout) {
        return socketTimeout(_socketTimeout);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder TcpNoDelay(bool _tcpNoDelay) {
        return tcpNoDelay(_tcpNoDelay);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder ValueSizeEstimate(int _valueSizeEstimate) {
        return valueSizeEstimate(_valueSizeEstimate);
    }
    
    public Infinispan.HotRod.SWIG.ConfigurationBuilder MaxRetries(int _maxRetries) {
        return maxRetries(_maxRetries);
    }
    
    public Infinispan.HotRod.SWIG.ClusterConfigurationBuilder AddCluster(string _clusterName) {
        return addCluster(_clusterName);
    }

    public Infinispan.HotRod.SWIG.ConfigurationBuilder BalancingStrategyProducer(Infinispan.HotRod.Config.FailOverRequestBalancingStrategyProducerDelegate d)
    {
            id = delegate () { return (new InternalFailOverRequestBalancingStrategy(d())).myHandle(); };
            balancingStrategyProducer(id);
            return this;
    }

    public Infinispan.HotRod.SWIG.ConfigurationBuilder SetTransactional(bool t) {
        return setTransactional(t);
    }
    
    %}

%typemap(csinterfaces_derived) infinispan::hotrod::ServerConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ServerConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ServerConfigurationBuilder %{

    public Infinispan.HotRod.SWIG.ServerConfiguration Create() {
        return create();
    }
    
    public Infinispan.HotRod.SWIG.ServerConfigurationBuilder Host(String _host) {
        return host(_host);
    }
    
    public Infinispan.HotRod.SWIG.ServerConfigurationBuilder Port(int _port) {
        return port(_port);
    }
    %}

%typemap(cscode) infinispan::hotrod::FailOverRequestBalancingStrategy %{
        public IntPtr myHandle()
        {

            return swigCPtr.Handle;
        }
%}

%typemap(csinterfaces_derived) infinispan::hotrod::ConnectionPoolConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ConnectionPoolConfigurationBuilder %{
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfiguration Create() {
        return create();
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder ExhaustedAction(Infinispan.HotRod.Config.ExhaustedAction _exhaustedAction) {
        return exhaustedAction((int) _exhaustedAction);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder Lifo(bool _lifo) {
        return lifo(_lifo);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MaxActive(int _maxActive) {
        return maxActive(_maxActive);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MaxTotal(int _maxTotal) {
        return maxTotal(_maxTotal);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MaxWait(int _maxWait) {
        return maxWait(_maxWait);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MaxIdle(int _maxIdle) {
        return maxIdle(_maxIdle);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MinIdle(int _minIdle) {
        return minIdle(_minIdle);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder NumTestsPerEvictionRun(int _numTestsPerEvictionRun) {
        return numTestsPerEvictionRun(_numTestsPerEvictionRun);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder TimeBetweenEvictionRuns(int _timeBetweenEvictionRuns) {
        return timeBetweenEvictionRuns(_timeBetweenEvictionRuns);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder MinEvictableIdleTime(int _minEvictableIdleTime) {
        return minEvictableIdleTime(_minEvictableIdleTime);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder TestOnBorrow(bool _testOnBorrow) {
        return testOnBorrow(_testOnBorrow);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder TestOnReturn(bool _testOnReturn) {
        return testOnReturn(_testOnReturn);
    }
    
    public Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder TestWhileIdle(bool _testWhileIdle) {
        return testWhileIdle(_testWhileIdle);
    }
    %}
%extend infinispan::hotrod::ConnectionPoolConfigurationBuilder {
    infinispan::hotrod::ConnectionPoolConfigurationBuilder& exhaustedAction(int val) {
        infinispan::hotrod::ExhaustedAction exhaustedAction;
        switch (val) {
        case 0:
            exhaustedAction = infinispan::hotrod::ExhaustedAction::EXCEPTION;
            break;
        case 1:
            exhaustedAction = infinispan::hotrod::ExhaustedAction::WAIT;
            break;
        case 2:
            exhaustedAction = infinispan::hotrod::ExhaustedAction::CREATE_NEW;
            break;
        default:
            std::stringstream out;
            out << "Cannot map to ExhaustedAction: " << val;
            std::cout << out.str() << std::endl;
            throw infinispan::hotrod::Exception(out.str());
        }

        $self->exhaustedAction(exhaustedAction);

        return *$self;
    }
};

%typemap(csinterfaces_derived) infinispan::hotrod::SslConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.SslConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::SslConfigurationBuilder %{

    public Infinispan.HotRod.SWIG.SslConfiguration Create() {
        return create();
    }

    public Infinispan.HotRod.SWIG.SslConfigurationBuilder Enable() {
        return enable();
    }

    public Infinispan.HotRod.SWIG.SslConfigurationBuilder ServerCAFile(string filename) {
        return serverCAFile(filename);
    }

    public Infinispan.HotRod.SWIG.SslConfigurationBuilder ClientCertificateFile(string filename) {
        return clientCertificateFile(filename);
    }

    public Infinispan.HotRod.SWIG.SslConfigurationBuilder SniHostName(string _sniHostName) {
        return sniHostName(_sniHostName);
    }

    public string GetSniHostName() {
       return getSniHostName();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::AuthenticationConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::AuthenticationConfigurationBuilder %{

    public Infinispan.HotRod.SWIG.AuthenticationConfiguration Create() {
        return create();
    }

    public Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder Enable() {
        return enable();
    }

    public Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder Disable() {
        return disable();
    }

    public Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder SaslMechanism(string saslMechanism) {
      return this.saslMechanism(saslMechanism);
    }

    public Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder ServerFQDN(string serverFQDN) {
      return this.serverFQDN(serverFQDN);
    }


    %}

%typemap(csinterfaces_derived) infinispan::hotrod::SecurityConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.SecurityConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::SecurityConfigurationBuilder %{

    public Infinispan.HotRod.SWIG.SecurityConfiguration Create() {
        return create();
    }

    public Infinispan.HotRod.SWIG.AuthenticationConfigurationBuilder Authentication() {
        return authentication();
    }

    %}

%typemap(csinterfaces_derived) infinispan::hotrod::NearCacheConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::NearCacheConfigurationBuilder %{

    public Infinispan.HotRod.SWIG.NearCacheConfiguration Create() {
        return create();
    }

    public Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder Mode(Infinispan.HotRod.NearCacheMode _mode) {
        switch(_mode) {
              case Infinispan.HotRod.NearCacheMode.INVALIDATED: return mode(NearCacheMode.INVALIDATED);
              case Infinispan.HotRod.NearCacheMode.DISABLED:
              default: return mode(NearCacheMode.DISABLED);
        }
    }

    public Infinispan.HotRod.NearCacheMode GetMode() {
        switch(getMode()) {
              case NearCacheMode.INVALIDATED: return Infinispan.HotRod.NearCacheMode.INVALIDATED;
              case NearCacheMode.DISABLED:
              default: return Infinispan.HotRod.NearCacheMode.DISABLED;
        }
    }

    public Infinispan.HotRod.SWIG.NearCacheConfigurationBuilder MaxEntries(int maxEntries) {
        return this.maxEntries((uint)maxEntries);
    }

    public int GetMaxEntries() {
        return getMaxEntries();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::ClusterConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ClusterConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ClusterConfigurationBuilder %{
public Infinispan.HotRod.SWIG.ClusterConfigurationBuilder AddClusterNode(string host, int port) {
       return addClusterNode(host, port);
    }
%}
%typemap(csinterfaces) infinispan::hotrod::Configuration "IDisposable, Infinispan.HotRod.SWIG.Configuration"
%typemap(cscode) infinispan::hotrod::Configuration %{

FailOverRequestBalancingStrategyProducerDelegate failOverStrategyProducerDelegate;
internal void balancingStrategyProducer(FailOverRequestBalancingStrategyProducerDelegate failOverStrategyProducerDelegate)
{
  this.failOverStrategyProducerDelegate = failOverStrategyProducerDelegate;
}

public System.Collections.Generic.IList<Infinispan.HotRod.SWIG.ServerConfiguration> Servers() {
        System.Collections.Generic.List<Infinispan.HotRod.SWIG.ServerConfiguration> result
            = new System.Collections.Generic.List<Infinispan.HotRod.SWIG.ServerConfiguration>();
        ServerConfigurationVector serversVec;
        getServersMapConfiguration().TryGetValue("DEFAULT_CLUSTER_NAME",out serversVec);  
        foreach (Infinispan.HotRod.SWIG.ServerConfiguration config in serversVec) {
            result.Add(config);
        }
        return result;
    }
public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Infinispan.HotRod.SWIG.ServerConfiguration>> GetServersMapConfiguration() {
  System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Infinispan.HotRod.SWIG.ServerConfiguration>> dic = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IList<Infinispan.HotRod.SWIG.ServerConfiguration>>();
  foreach (System.Collections.Generic.KeyValuePair<string, ServerConfigurationVector> p in getServersMapConfiguration())
  {
    System.Collections.Generic.IList<SWIG.ServerConfiguration> vec = new System.Collections.Generic.List<SWIG.ServerConfiguration>();
    foreach (SWIG.ServerConfiguration sc in p.Value)
    {
      vec.Add(sc);
    }
    dic.Add(p.Key, vec);
  }
  return dic;
}
    public Infinispan.HotRod.SWIG.ConnectionPoolConfiguration ConnectionPool() {
        return getConnectionPoolConfiguration();
    }

    public Infinispan.HotRod.SWIG.SslConfiguration Ssl() {
        return getSslConfiguration();
    }

    public Infinispan.HotRod.SWIG.NearCacheConfiguration NearCache() {
        return getNearCacheConfiguration();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::ServerConfiguration "IDisposable, Infinispan.HotRod.SWIG.ServerConfiguration"
%typemap(csinterfaces) infinispan::hotrod::SslConfiguration "IDisposable, Infinispan.HotRod.SWIG.SslConfiguration"
%typemap(csinterfaces) infinispan::hotrod::AuthenticationConfiguration "IDisposable, Infinispan.HotRod.SWIG.AuthenticationConfiguration"
%typemap(csinterfaces) infinispan::hotrod::SecurityConfiguration "IDisposable, Infinispan.HotRod.SWIG.SecurityConfiguration"
%typemap(csinterfaces) infinispan::hotrod::NearCacheConfiguration "IDisposable, Infinispan.HotRod.SWIG.NearCacheConfiguration"

%typemap(csinterfaces) infinispan::hotrod::ConnectionPoolConfiguration "IDisposable, Infinispan.HotRod.SWIG.ConnectionPoolConfiguration"
%typemap(cscode) infinispan::hotrod::ConnectionPoolConfiguration %{
    public Infinispan.HotRod.Config.ExhaustedAction ExhaustedAction() {
        return (Infinispan.HotRod.Config.ExhaustedAction) (int) getExhaustedAction();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::RemoteCache<infinispan::hotrod::ByteArray, infinispan::hotrod::ByteArray> "IDisposable, Infinispan.HotRod.SWIG.RemoteByteArrayCache"

%typemap(csinterfaces) infinispan::hotrod::RemoteCacheManager "IDisposable, Infinispan.HotRod.SWIG.RemoteCacheManager"
%typemap(csinterfaces) infinispan::hotrod::TransactionManager "IDisposable, Infinispan.HotRod.SWIG.TransactionManager"
%typemap(csinterfaces) infinispan::hotrod::RemoteCounterManager "IDisposable, Infinispan.HotRod.RemoteCounterManager"
%typemap(csinterfaces) infinispan::hotrod::Counter "IDisposable, Infinispan.HotRod.Counter"
%typemap(csinterfaces_derived) infinispan::hotrod::StrongCounter "IDisposable, Infinispan.HotRod.StrongCounter"
%typemap(csinterfaces_derived) infinispan::hotrod::WeakCounter "IDisposable, Infinispan.HotRod.WeakCounter"
%typemap(csinterfaces) infinispan::hotrod::CounterConfiguration "IDisposable, Infinispan.HotRod.ICounterConfiguration"

%typemap(cscode) infinispan::hotrod::TransactionManager %{
    public void Begin() {
        begin();
    }

    public void Commit() {
        commit();
    }

    public void Rollback() {
        rollback();
    }
%}
%typemap(cscode) infinispan::hotrod::RemoteCacheManager %{
    public void Start() {
        start();
    }

    public void Stop() {
        stop();
    }

    public bool IsStarted() {
        return isStarted();
    }

    public Infinispan.HotRod.SWIG.RemoteByteArrayCache GetByteArrayCache() {
        return (RemoteByteArrayCache) getByteArrayCache();
    }
    
    public Infinispan.HotRod.SWIG.RemoteByteArrayCache GetByteArrayCache(String cacheName) {
        return (RemoteByteArrayCache) getByteArrayCache(cacheName);
    }

    public Infinispan.HotRod.SWIG.RemoteByteArrayCache GetByteArrayCache(bool forceReturnValue) {
        return (RemoteByteArrayCache) getByteArrayCache(forceReturnValue);
    }

    public Infinispan.HotRod.SWIG.RemoteByteArrayCache GetByteArrayCache(String cacheName, bool forceReturnValue) {
        return (RemoteByteArrayCache) getByteArrayCache(cacheName, forceReturnValue);
    }

    public bool SwitchToCluster(string clusterName) {
        return switchToCluster(clusterName);
    }

    public bool SwitchToDefaultCluster() {
        return switchToDefaultCluster();
    }

    public Infinispan.HotRod.RemoteCounterManager GetCounterManager() {
        return getCounterManager();
    }

    public System.Collections.Generic.ISet<string> GetCacheNames() {
            System.Collections.Generic.ISet<string> retSet = new System.Collections.Generic.HashSet<string>();
            var v = getCacheNames();
            var it = v.create_iterator_begin();
            while (v.has_next(it))
            {
                retSet.Add(v.get_next_key(it));
            }
            v.destroy_iterator(it);
        return retSet;
    }

    public Infinispan.HotRod.SWIG.TransactionManager GetTransactionManager() {
        return (Infinispan.HotRod.SWIG.TransactionManager)getTransactionManager();
    }

%}

%typemap(cscode) infinispan::hotrod::RemoteCounterManager %{
    public Infinispan.HotRod.StrongCounter GetStrongCounter(string name) {
        return getStrongCounter(name);
    }
    public Infinispan.HotRod.WeakCounter GetWeakCounter(string name) {
        return getWeakCounter(name);
    }
    public bool DefineCounter(string name, Infinispan.HotRod.ICounterConfiguration conf) {
        CounterConfiguration swigConf = new CounterConfiguration(conf.GetInitialValue(), conf.GetLowerBound(), conf.GetUpperBound(),
                                      conf.GetConcurrencyLevel(), (CounterType) conf.GetType(), 
                                      (Storage) conf.GetStorage());
        return defineCounter(name, swigConf);
    }
    public bool IsDefined(string name) {
        return isDefined(name);
    }
    public Infinispan.HotRod.ICounterConfiguration GetConfiguration(string counterName) {
        return getConfiguration(counterName);
    }
    public void Remove(string counterName) {
        remove(counterName);
    }
%}

%typemap(cscode) infinispan::hotrod::CounterConfiguration %{
    public int GetInitialValue() {
        return getInitialValue();
    }
    public int GetUpperBound() {
        return getUpperBound();
    }
    public int GetLowerBound() {
        return getLowerBound();
    }
    public Infinispan.HotRod.CounterType GetType() {
        return (Infinispan.HotRod.CounterType)getType();
    }
    public int GetConcurrencyLevel() {
        return getConcurrencyLevel();
    }
    public Infinispan.HotRod.Storage GetStorage() {
        return (Infinispan.HotRod.Storage)getStorage();
    }
    public string ToString() {
        return toString();
    }
%}


%typemap(cscode) infinispan::hotrod::Counter %{
    public string GetName() {
        return getName();
    }
    public Infinispan.HotRod.ICounterConfiguration GetConfiguration() {
        return getConfiguration();
    } 
    public void Reset() {
        reset();
    }
    public void Remove() {
        remove();
    }
    public object AddListener(Infinispan.HotRod.Event.CounterListener listener) {
            return addListener(listener.cli);
    }
    public void RemoveListener(object handler) {
            removeListener((Infinispan.HotRod.SWIGGen.SWIGTYPE_p_void) handler);
    }
%}

%typemap(cscode) infinispan::hotrod::StrongCounter %{
    public int GetValue() {
        return getValue();
    }
    public int AddAndGet(int delta) {
        return addAndGet(delta);
    }
    public int IncrementAndGet() {
        return incrementAndGet();
    }
    public int DecrementAndGet() {
        return decrementAndGet();
    }
    public int CompareAndSwap(int expect, int update) {
        return compareAndSwap(expect, update);
    }
    public bool CompareAndSet(int expect, int update) {
        return compareAndSet(expect, update);
    }
%}

%typemap(cscode) infinispan::hotrod::WeakCounter %{
    public int GetValue() {
        return getValue();
    }
    public void Add(int delta) {
        add(delta);
    }
    public void Increment() {
        increment();
    }
    public void Decrement() {
        decrement();
    }
%}

%typemap(cscode) infinispan::hotrod::RemoteCacheManagerAdmin %{
        public void withFlags(System.Collections.Generic.ISet<Infinispan.HotRod.AdminFlag> flags)
        {
            var aSet = new AdminFlagSet();
            foreach(var i in flags)
            {
                switch(i)
                {
                    case Infinispan.HotRod.AdminFlag.PERMANENT:
                        aSet.setitem(Infinispan.HotRod.SWIGGen.AdminFlag.PERMANENT);
                        break;
                }
            }
            withFlags(aSet);
        }
    %}
