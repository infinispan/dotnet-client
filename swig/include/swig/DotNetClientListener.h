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
#include "swig/Queue.h"
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
    ClientCacheEventData(void *) : eventType(0xff) {}
    uint8_t eventType;
    bool isCustom;
    std::vector<char> listenerId;
    std::vector<char> key;
    uint64_t version;
    bool isCommandRetried;
    std::vector<char> data;
};

class DotNetClientListener : public ClientListener
{
public:
  std::function<void()> getFailoverFunction()
  {
    auto &cQ = this->q;
    auto &cListenerId = this->getListenerId();
    return [cQ, cListenerId](){
    ClientCacheEventData eData;
    eData.eventType=5;
    eData.listenerId=cListenerId;
    cQ->push(eData);
    };
  }

  ClientCacheEventData pop()
  {
    return q->pop();
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
    const_cast<DotNetClientListener*>(this)->q->push(eData);
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
    const_cast<DotNetClientListener*>(this)->q->push(eData);
  }
  virtual void processEvent(ClientCacheEntryRemovedEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    const_cast<DotNetClientListener*>(this)->q->push(eData);
  }
  virtual void processEvent(ClientCacheEntryExpiredEvent<std::vector<char> > e, std::vector<char >listId, uint8_t isCustom) const
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.key=e.getKey();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    const_cast<DotNetClientListener*>(this)->q->push(eData);
  }
  virtual void processEvent(ClientCacheEntryCustomEvent e, std::vector<char >listId, uint8_t isCustom) const 
  {
    ClientCacheEventData eData;
    eData.isCustom=isCustom;
    eData.isCommandRetried=e.isCommandRetried();
    eData.data=e.getEventData();
    eData.eventType=e.getType();
    eData.listenerId=listId;
    const_cast<DotNetClientListener*>(this)->q->push(eData);
  }
  virtual void processFailoverEvent() const
  {
    ClientCacheEventData eData;
    const_cast<DotNetClientListener*>(this)->q->push(eData);
  }
  DotNetClientListener() : q(new Queue<ClientCacheEventData>()) {} 

  virtual ~DotNetClientListener() {}
  void setShutdown(bool v) { shutdown=v; }
  bool isShutdown() { return shutdown; }
private:
  std::shared_ptr<Queue<ClientCacheEventData> > q;
  bool shutdown=false;

};

}}}


#endif /* INCLUDE_INFINISPAN_HOTROD_DOTNETCLIENTLISTENER_H_ */
