package org.infinispan.event.cpp.tutorial;

import org.infinispan.filter.NamedFactory;
import org.infinispan.metadata.Metadata;
import org.infinispan.notifications.cachelistener.filter.CacheEventConverter;
import org.infinispan.notifications.cachelistener.filter.CacheEventConverterFactory;
import org.infinispan.notifications.cachelistener.filter.EventType;

@NamedFactory(name = "to-string-converter-factory")
public class ToStringEventConverterFactory implements CacheEventConverterFactory {

	@Override
	public CacheEventConverter<String, String, String> getConverter(Object[] params) {
		// TODO Auto-generated method stub
		return new ToStringConverter();
	}
	class ToStringConverter implements CacheEventConverter<String, String, String>  {
		      public String convert(String key, String oldValue, Metadata oldMetadata, 
		               String newValue, Metadata newMetadata, EventType eventType) {
		         return "custom event: "+ key+" "+newValue;
		      }
		}

}
