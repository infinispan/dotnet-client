/*
 * DotNetClientListener.h
 *
 *  Created on: Aug 3, 2016
 *      Author: rigazilla
 */

#ifndef INCLUDE_INFINISPAN_HOTROD_DOTNETCLIENTLISTENER_H_
#define INCLUDE_INFINISPAN_HOTROD_DOTNETCLIENTLISTENER_H_

#include "infinispan/hotrod/ClientEvent.h"
#include "infinispan/hotrod/ImportExport.h"
#include <vector>
#include <list>
#include <functional>

using namespace infinispan::hotrod;

namespace infinispan {
namespace hotrod {

namespace transport {
class Transport;
}

namespace protocol {
class Codec20;
}
class RemoteCacheBase;
template <class K, class V>
class RemoteCache;

namespace event {

class ClientCacheEventData {
public:
    ClientCacheEventData() {}
    ClientCacheEventData(const std::vector<char>& cData) : data(cData.begin(),cData.end()) {}
    ClientCacheEventData(void *) : eventType(0xff) {}
    uint8_t eventType;
    bool isCustom;
    std::vector<char> listenerId;
    std::vector<char> key;
    uint64_t version;
    bool isCommandRetried;
    std::vector<unsigned char> data;
    std::vector<unsigned char>& getData() { return data; }
};

class ClientListenerCallback {
public:
    virtual ~ClientListenerCallback() {}
    virtual void processEvent(ClientCacheEventData& evData)
    {
        return;
    }
};


class DotNetClientListener : public ClientListener
{
public:
  std::vector<unsigned char>& getUListenerId()
  {
    uListenerId.clear();
    for (auto c : this->getListenerId())
    {
       uListenerId.push_back((unsigned char) c);
    }
    return uListenerId;
  }

  std::function<void()> getFailoverFunction()
  {
    auto &cListenerId = this->getListenerId();
    return [this, cListenerId](){
    ClientCacheEventData eData;
    eData.eventType=5;
    eData.listenerId=cListenerId;
    this->cb->processEvent(eData);
    };
  }
  
  virtual void processEvent(ClientCacheEntryCreatedEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    eData.version=e.getVersion();
    this->cb->processEvent(eData);
  }
  virtual void processEvent(ClientCacheEntryModifiedEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    eData.version=e.getVersion();
    this->cb->processEvent(eData);
  }
  virtual void processEvent(ClientCacheEntryRemovedEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    this->cb->processEvent(eData);
  }
  virtual void processEvent(ClientCacheEntryExpiredEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    this->cb->processEvent(eData);
  }
  virtual void processEvent(ClientCacheEntryCustomEvent e, std::vector<char >listId, uint8_t isCustom) const 
  {
    ClientCacheEventData eData(e.getEventData());
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.eventType=e.getType();
    eData.listenerId=(const std::vector<char>&)listId;
    this->cb->processEvent(eData);
  }
  virtual void processFailoverEvent() const
  {
    ClientCacheEventData eData;
    this->cb->processEvent(eData);
  }
  DotNetClientListener()  { } 

  virtual ~DotNetClientListener() { }
  void setShutdown(bool v) { shutdown=v; }
  bool isShutdown() { return shutdown; }
  void setCb(ClientListenerCallback* cb)
  {
    this->cb=cb;
  }
  ClientListenerCallback* getCb()
  {
    return cb;
  }
private:
  ClientListenerCallback* cb=nullptr;
  bool shutdown=false;
  std::vector<unsigned char> uListenerId;

};

}}}


#endif /* INCLUDE_INFINISPAN_HOTROD_DOTNETCLIENTLISTENER_H_ */
