using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    [Collection("MainSequence")]
    public class AuthorizationScramSha256Test : BaseAuthorizationTest
    {
        public AuthorizationScramSha256Test(AuthorizationCacheTestFixture fixture) : base(fixture)
        {
        }

        public override string GetMech()
        {
            return "SCRAM-SHA-256";
        }

        [Fact]
        public void ReaderSuccessTest()
        {
            tester.TestReaderSuccess(readerCache);
        }

        [Fact]
        public void ReaderPerformsWritesTest()
        {
            tester.TestReaderPerformsWrites(readerCache);
        }

        [Fact]
        public void WriterSuccessTest()
        {
            tester.TestWriterSuccess(writerCache);
        }

        [Fact]
        public void WriterPerformsReadsTest()
        {
            tester.TestWriterPerformsReads(writerCache);
        }

        [Fact]
        public void WriterPerformsSupervisorOpsTest()
        {
            tester.TestWriterPerformsSupervisorOps(writerCache, scriptCache);
        }

        [Fact]
        public void SupervisorSuccessTest()
        {
            tester.TestSupervisorSuccess(supervisorCache, scriptCache);
        }

        [Fact]
        public void SupervisorPerformsAdminOpsTest()
        {
            tester.TestSupervisorPerformsAdminOps(supervisorCache);
        }

        [Fact]
        public void AdminSuccessTest()
        {
            tester.TestAdminSuccess(adminCache, scriptCache);
        }

        [Fact]
        public void ReaderCannotAccessStatsTest()
        {
            Assert.Throws<InfinispanException>(() => tester.TestReaderAccessStats(readerCache, scriptCache));
        }
    }
}
