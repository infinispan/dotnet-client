Content:
  1. Prerequisite
  2. Content
  3. Running the sample application
  4. Feedback
  5. Source Repository

1. Prerequisite
   - the Infinispan Hot Rod .NET client library requires Microsoft .NET Framework 4 which can be downloaded from here: http://www.microsoft.com/en-us/download/details.aspx?id=17851
   - Microsoft Visual C# 2010 Express (or Visual Studio 2010) is needed in order to run the sample application. The former is available here:  http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express
   - Infinispan Hot Rod server need to be downloaded and installed (see section 3)

2. Content
   - bin: contains client's .dll together with the required dependencies 
     It also contains the API documentation (infinispanclient.xml) and program database files required for debugging (infinispanclient.pdb)
   - config: sample configuration files
   - sample: a sample application together with the Microsoft Visual Studion files required in order to run it (see section 3)

3. Running the sample application
   - in order to be able to run the sample application you’ll need to download the Infinispan server. An Infinispan server distribution can be found here: http://www.jboss.org/infinispan/downloads.html
   - unzip the distribution in a folder. We’ll refer to it as <ISPN_HOME>
   - start a hotrod server by running “"bin\startServer.bat -r hotrod" from the <ISPN_HOME> directory. (for more details see: https://docs.jboss.org/author/display/ISPN/Using+Hot+Rod+Server)
   - open the Microsoft Visual C# 2010 Express the "sample.csproj" file located in the “sample” folder of the infinispan dot-net client distribution
   - run the SampleInfinispanClientApplication

4. Feedback
   Any feedback is much appreciated! Feel free to add your comments on the forums[1], mailing lists[2] or contact us directly on the IRC[3]  
   [1] https://community.jboss.org/en/infinispan?view=discussions
   [2] http://www.jboss.org/infinispan/mailinglists
   [3] irc://irc.freenode.org/infinispan

5. Source Repository
   The InfinispanDotNetClient is an open source project. In order to fetch the sources you need git: https://help.github.com/articles/set-up-git
   Sources are located here: https://github.com/infinispan/dotnet-client