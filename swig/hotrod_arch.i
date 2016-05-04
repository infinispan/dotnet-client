%typemap(csinterfaces_derived) infinispan::hotrod::ConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ConfigurationBuilder %{
    public void Read(Infinispan.HotRod.SWIG.Configuration bean) {
        read((Configuration) bean);
    }

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
    
    %}

%typemap(csinterfaces_derived) infinispan::hotrod::ServerConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ServerConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ServerConfigurationBuilder %{
    public void Read(Infinispan.HotRod.SWIG.ServerConfiguration bean) {
        read((ServerConfiguration) bean);
    }

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

%typemap(csinterfaces_derived) infinispan::hotrod::ConnectionPoolConfigurationBuilder "IDisposable, Infinispan.HotRod.SWIG.ConnectionPoolConfigurationBuilder"
%typemap(cscode) infinispan::hotrod::ConnectionPoolConfigurationBuilder %{
    public void Read(Infinispan.HotRod.SWIG.ConnectionPoolConfiguration bean) {
        read((ConnectionPoolConfiguration) bean);
    }
    
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
    public void Read(Infinispan.HotRod.SWIG.SslConfiguration bean) {
        read((SslConfiguration) bean);
    }

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
    %}

%typemap(csinterfaces) infinispan::hotrod::Configuration "IDisposable, Infinispan.HotRod.SWIG.Configuration"
%typemap(cscode) infinispan::hotrod::Configuration %{
    public System.Collections.Generic.IList<Infinispan.HotRod.SWIG.ServerConfiguration> Servers() {
        System.Collections.Generic.List<Infinispan.HotRod.SWIG.ServerConfiguration> result
            = new System.Collections.Generic.List<Infinispan.HotRod.SWIG.ServerConfiguration>();

        foreach (Infinispan.HotRod.SWIG.ServerConfiguration config in getServersConfiguration()) {
            result.Add(config);
        }
        return result;
    }

    public Infinispan.HotRod.SWIG.ConnectionPoolConfiguration ConnectionPool() {
        return getConnectionPoolConfiguration();
    }

    public Infinispan.HotRod.SWIG.SslConfiguration Ssl() {
        return getSslConfiguration();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::ServerConfiguration "IDisposable, Infinispan.HotRod.SWIG.ServerConfiguration"
%typemap(csinterfaces) infinispan::hotrod::SslConfiguration "IDisposable, Infinispan.HotRod.SWIG.SslConfiguration"

%typemap(csinterfaces) infinispan::hotrod::ConnectionPoolConfiguration "IDisposable, Infinispan.HotRod.SWIG.ConnectionPoolConfiguration"
%typemap(cscode) infinispan::hotrod::ConnectionPoolConfiguration %{
    public Infinispan.HotRod.Config.ExhaustedAction ExhaustedAction() {
        return (Infinispan.HotRod.Config.ExhaustedAction) (int) getExhaustedAction();
    }
    %}

%typemap(csinterfaces) infinispan::hotrod::RemoteCache<infinispan::hotrod::ByteArray, infinispan::hotrod::ByteArray> "IDisposable, Infinispan.HotRod.SWIG.RemoteByteArrayCache"

%typemap(csinterfaces) infinispan::hotrod::RemoteCacheManager "IDisposable, Infinispan.HotRod.SWIG.RemoteCacheManager"
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
    %}
