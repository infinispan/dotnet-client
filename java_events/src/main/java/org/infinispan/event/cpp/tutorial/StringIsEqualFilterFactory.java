package org.infinispan.event.cpp.tutorial;

import java.io.Serializable;

import org.infinispan.filter.NamedFactory;
import org.infinispan.metadata.Metadata;
import org.infinispan.notifications.cachelistener.filter.CacheEventFilter;
import org.infinispan.notifications.cachelistener.filter.CacheEventFilterFactory;
import org.infinispan.notifications.cachelistener.filter.EventType;

@NamedFactory(name = "string-is-equal-filter-factory")
public class StringIsEqualFilterFactory implements CacheEventFilterFactory {

	@Override
	public CacheEventFilter<String, String> getFilter(Object[] params) {
		// TODO Auto-generated method stub
		return new StringIsEqualFilter(params);
	}

	public class StringIsEqualFilter implements CacheEventFilter<String, String>, Serializable {
		private static final long serialVersionUID = 1L;
        private String productName;
		public StringIsEqualFilter(Object[] params) {
			productName=(String)params[0];
			System.out.println("Created with par "+productName);
		}

		@Override
		public boolean accept(String key, String oldValue, Metadata oldMetadata, String newValue, Metadata newMetadata,
				EventType eventType) {
			System.out.println("Checking if "+key+" starts with "+productName);
			System.out.println("Result is: "+(key!=null && key.startsWith(productName)));
			return key!=null && key.startsWith(productName);
		}
	}
}
