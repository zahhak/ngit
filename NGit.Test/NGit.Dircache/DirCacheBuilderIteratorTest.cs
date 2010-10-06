using NGit;
using NGit.Dircache;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using Sharpen;

namespace NGit.Dircache
{
	public class DirCacheBuilderIteratorTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestPathFilterGroup_DoesNotSkipTail()
		{
			DirCache dc = db.ReadDirCache();
			FileMode mode = FileMode.REGULAR_FILE;
			string[] paths = new string[] { "a.", "a/b", "a/c", "a/d", "a0b" };
			DirCacheEntry[] ents = new DirCacheEntry[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				ents[i] = new DirCacheEntry(paths[i]);
				ents[i].SetFileMode(mode);
			}
			{
				DirCacheBuilder b = dc.Builder();
				for (int i_1 = 0; i_1 < ents.Length; i_1++)
				{
					b.Add(ents[i_1]);
				}
				b.Finish();
			}
			int expIdx = 2;
			DirCacheBuilder b_1 = dc.Builder();
			TreeWalk tw = new TreeWalk(db);
			tw.Reset();
			tw.AddTree(new DirCacheBuildIterator(b_1));
			tw.Recursive = true;
			tw.Filter = PathFilterGroup.CreateFromStrings(Collections.Singleton(paths[expIdx]
				));
			NUnit.Framework.Assert.IsTrue("found " + paths[expIdx], tw.Next());
			DirCacheIterator c = tw.GetTree<DirCacheIterator>(0);
			NUnit.Framework.Assert.IsNotNull(c);
			NUnit.Framework.Assert.AreEqual(expIdx, c.ptr);
			NUnit.Framework.Assert.AreSame(ents[expIdx], c.GetDirCacheEntry());
			NUnit.Framework.Assert.AreEqual(paths[expIdx], tw.PathString);
			NUnit.Framework.Assert.AreEqual(mode.GetBits(), tw.GetRawMode(0));
			NUnit.Framework.Assert.AreSame(mode, tw.GetFileMode(0));
			b_1.Add(c.GetDirCacheEntry());
			NUnit.Framework.Assert.IsFalse("no more entries", tw.Next());
			b_1.Finish();
			NUnit.Framework.Assert.AreEqual(ents.Length, dc.GetEntryCount());
			for (int i_2 = 0; i_2 < ents.Length; i_2++)
			{
				NUnit.Framework.Assert.AreSame(ents[i_2], dc.GetEntry(i_2));
			}
		}
	}
}