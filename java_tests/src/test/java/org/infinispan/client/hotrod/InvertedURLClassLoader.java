package org.infinispan.client.hotrod;

import java.util.List;
import java.net.URL;
import java.net.URLClassLoader;

public class InvertedURLClassLoader extends URLClassLoader {
    public InvertedURLClassLoader(URL[] urls) {
        super(urls, null); // this classloader has no parent
    }

    protected Class<?> findClass(String name) throws ClassNotFoundException {
        try {
            /* Try to find the class in the java hotrod client jar first. */
            return super.findClass(name);
        } catch (ClassNotFoundException ex) {
            /* Delegate to the main CL if it's not a class from that jar. */
            return getClass().getClassLoader().loadClass(name);
        }
    }
}
