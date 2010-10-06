using System.Collections.Generic;
using NGit;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	public class TransportTest : SampleDataRepositoryTestCase
	{
		private NGit.Transport.Transport transport;

		private RemoteConfig remoteConfig;

		/// <exception cref="System.Exception"></exception>
		protected override void SetUp()
		{
			base.SetUp();
			Config config = ((FileBasedConfig)db.GetConfig());
			remoteConfig = new RemoteConfig(config, "test");
			remoteConfig.AddURI(new URIish("http://everyones.loves.git/u/2"));
			transport = null;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void TearDown()
		{
			if (transport != null)
			{
				transport.Close();
				transport = null;
			}
			base.TearDown();
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion with simple RefSpec - no
		/// wildcard, no tracking ref in repo configuration.
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion with simple RefSpec - no
		/// wildcard, no tracking ref in repo configuration.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestFindRemoteRefUpdatesNoWildcardNoTracking()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Collections
				.NCopies(1, new RefSpec("refs/heads/master:refs/heads/x")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			RemoteRefUpdate rru = result.Iterator().Next();
			NUnit.Framework.Assert.IsNull(rru.GetExpectedOldObjectId());
			NUnit.Framework.Assert.IsFalse(rru.IsForceUpdate());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetSrcRef());
			AssertEquals(db.Resolve("refs/heads/master"), rru.GetNewObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/x", rru.GetRemoteName());
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion with no-destination RefSpec
		/// (destination should be set up for the same name as source).
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion with no-destination RefSpec
		/// (destination should be set up for the same name as source).
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestFindRemoteRefUpdatesNoWildcardNoDestination()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/master")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			RemoteRefUpdate rru = result.Iterator().Next();
			NUnit.Framework.Assert.IsNull(rru.GetExpectedOldObjectId());
			NUnit.Framework.Assert.IsTrue(rru.IsForceUpdate());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetSrcRef());
			AssertEquals(db.Resolve("refs/heads/master"), rru.GetNewObjectId());
			NUnit.Framework.Assert.AreEqual("refs/heads/master", rru.GetRemoteName());
		}

		/// <summary>Test RefSpec to RemoteRefUpdate conversion with wildcard RefSpec.</summary>
		/// <remarks>Test RefSpec to RemoteRefUpdate conversion with wildcard RefSpec.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestFindRemoteRefUpdatesWildcardNoTracking()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/*:refs/heads/test/*")));
			NUnit.Framework.Assert.AreEqual(12, result.Count);
			bool foundA = false;
			bool foundB = false;
			foreach (RemoteRefUpdate rru in result)
			{
				if ("refs/heads/a".Equals(rru.GetSrcRef()) && "refs/heads/test/a".Equals(rru.GetRemoteName
					()))
				{
					foundA = true;
				}
				if ("refs/heads/b".Equals(rru.GetSrcRef()) && "refs/heads/test/b".Equals(rru.GetRemoteName
					()))
				{
					foundB = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(foundA);
			NUnit.Framework.Assert.IsTrue(foundB);
		}

		/// <summary>
		/// Test RefSpec to RemoteRefUpdate conversion for more than one RefSpecs
		/// handling.
		/// </summary>
		/// <remarks>
		/// Test RefSpec to RemoteRefUpdate conversion for more than one RefSpecs
		/// handling.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestFindRemoteRefUpdatesTwoRefSpecs()
		{
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			RefSpec specA = new RefSpec("+refs/heads/a:refs/heads/b");
			RefSpec specC = new RefSpec("+refs/heads/c:refs/heads/d");
			ICollection<RefSpec> specs = Arrays.AsList(specA, specC);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(specs);
			NUnit.Framework.Assert.AreEqual(2, result.Count);
			bool foundA = false;
			bool foundC = false;
			foreach (RemoteRefUpdate rru in result)
			{
				if ("refs/heads/a".Equals(rru.GetSrcRef()) && "refs/heads/b".Equals(rru.GetRemoteName
					()))
				{
					foundA = true;
				}
				if ("refs/heads/c".Equals(rru.GetSrcRef()) && "refs/heads/d".Equals(rru.GetRemoteName
					()))
				{
					foundC = true;
				}
			}
			NUnit.Framework.Assert.IsTrue(foundA);
			NUnit.Framework.Assert.IsTrue(foundC);
		}

		/// <summary>Test RefSpec to RemoteRefUpdate conversion for tracking ref search.</summary>
		/// <remarks>Test RefSpec to RemoteRefUpdate conversion for tracking ref search.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void TestFindRemoteRefUpdatesTrackingRef()
		{
			remoteConfig.AddFetchRefSpec(new RefSpec("refs/heads/*:refs/remotes/test/*"));
			transport = NGit.Transport.Transport.Open(db, remoteConfig);
			ICollection<RemoteRefUpdate> result = transport.FindRemoteRefUpdatesFor(Sharpen.Collections
				.NCopies(1, new RefSpec("+refs/heads/a:refs/heads/a")));
			NUnit.Framework.Assert.AreEqual(1, result.Count);
			TrackingRefUpdate tru = result.Iterator().Next().GetTrackingRefUpdate();
			NUnit.Framework.Assert.AreEqual("refs/remotes/test/a", tru.GetLocalName());
			NUnit.Framework.Assert.AreEqual("refs/heads/a", tru.GetRemoteName());
			AssertEquals(db.Resolve("refs/heads/a"), tru.GetNewObjectId());
			NUnit.Framework.Assert.IsNull(tru.GetOldObjectId());
		}
	}
}