using System.Collections.Generic;
using System.Text;
using NGit;
using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	public class FooterLineTest : RepositoryTestCase
	{
		public virtual void TestNoFooters_EmptyBody()
		{
			RevCommit commit = Parse(string.Empty);
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_NewlineOnlyBody1()
		{
			RevCommit commit = Parse("\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_NewlineOnlyBody5()
		{
			RevCommit commit = Parse("\n\n\n\n\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_OneLineBodyNoLF()
		{
			RevCommit commit = Parse("this is a commit");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_OneLineBodyWithLF()
		{
			RevCommit commit = Parse("this is a commit\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_ShortBodyNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestNoFooters_ShortBodyWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(0, footers.Count);
		}

		public virtual void TestSignedOffBy_OneUserNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by: A. U. Thor <a@example.com>"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		public virtual void TestSignedOffBy_OneUserWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		public virtual void TestSignedOffBy_IgnoreWhitespace()
		{
			// We only ignore leading whitespace on the value, trailing
			// is assumed part of the value.
			//
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:   A. U. Thor <a@example.com>  \n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>  ", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
		}

		public virtual void TestEmptyValueNoLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:");
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual(string.Empty, f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		public virtual void TestEmptyValueWithLF()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Signed-off-by:\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual(string.Empty, f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		public virtual void TestShortKey()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "K:V\n");
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("K", f.GetKey());
			NUnit.Framework.Assert.AreEqual("V", f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		public virtual void TestNonDelimtedEmail()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Acked-by: re@example.com\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("re@example.com", f.GetValue());
			NUnit.Framework.Assert.AreEqual("re@example.com", f.GetEmailAddress());
		}

		public virtual void TestNotEmail()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "\n" + "Acked-by: Main Tain Er\n"
				);
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er", f.GetValue());
			NUnit.Framework.Assert.IsNull(f.GetEmailAddress());
		}

		public virtual void TestSignedOffBy_ManyUsers()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "Acked-by: Some Reviewer <sr@example.com>\n" + "Signed-off-by: Main Tain Er <mte@example.com>\n"
				);
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(4, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("a@example.com", f.GetEmailAddress());
			f = footers[1];
			NUnit.Framework.Assert.AreEqual("CC", f.GetKey());
			NUnit.Framework.Assert.AreEqual("<some.mailing.list@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("some.mailing.list@example.com", f.GetEmailAddress
				());
			f = footers[2];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Some Reviewer <sr@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("sr@example.com", f.GetEmailAddress());
			f = footers[3];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", f.GetValue());
			NUnit.Framework.Assert.AreEqual("mte@example.com", f.GetEmailAddress());
		}

		public virtual void TestSignedOffBy_SkipNonFooter()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-off-by: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "not really a footer line but we'll skip it anyway\n" + "Acked-by: Some Reviewer <sr@example.com>\n"
				 + "Signed-off-by: Main Tain Er <mte@example.com>\n");
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			FooterLine f;
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(4, footers.Count);
			f = footers[0];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", f.GetValue());
			f = footers[1];
			NUnit.Framework.Assert.AreEqual("CC", f.GetKey());
			NUnit.Framework.Assert.AreEqual("<some.mailing.list@example.com>", f.GetValue());
			f = footers[2];
			NUnit.Framework.Assert.AreEqual("Acked-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Some Reviewer <sr@example.com>", f.GetValue());
			f = footers[3];
			NUnit.Framework.Assert.AreEqual("Signed-off-by", f.GetKey());
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", f.GetValue());
		}

		public virtual void TestFilterFootersIgnoreCase()
		{
			RevCommit commit = Parse("subject\n\nbody of commit\n" + "Not-A-Footer-Line: this line must not be read as a footer\n"
				 + "\n" + "Signed-Off-By: A. U. Thor <a@example.com>\n" + "CC:            <some.mailing.list@example.com>\n"
				 + "Acked-by: Some Reviewer <sr@example.com>\n" + "signed-off-by: Main Tain Er <mte@example.com>\n"
				);
			// paragraph break, now footers appear in final block
			IList<string> footers = commit.GetFooterLines("signed-off-by");
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(2, footers.Count);
			NUnit.Framework.Assert.AreEqual("A. U. Thor <a@example.com>", footers[0]);
			NUnit.Framework.Assert.AreEqual("Main Tain Er <mte@example.com>", footers[1]);
		}

		public virtual void TestMatchesBugId()
		{
			RevCommit commit = Parse("this is a commit subject for test\n" + "\n" + "Simple-Bug-Id: 42\n"
				);
			// paragraph break, now footers appear in final block
			IList<FooterLine> footers = commit.GetFooterLines();
			NUnit.Framework.Assert.IsNotNull(footers);
			NUnit.Framework.Assert.AreEqual(1, footers.Count);
			FooterLine line = footers[0];
			NUnit.Framework.Assert.IsNotNull(line);
			NUnit.Framework.Assert.AreEqual("Simple-Bug-Id", line.GetKey());
			NUnit.Framework.Assert.AreEqual("42", line.GetValue());
			FooterKey bugid = new FooterKey("Simple-Bug-Id");
			NUnit.Framework.Assert.IsTrue("matches Simple-Bug-Id", line.Matches(bugid));
			NUnit.Framework.Assert.IsFalse("not Signed-off-by", line.Matches(FooterKey.SIGNED_OFF_BY
				));
			NUnit.Framework.Assert.IsFalse("not CC", line.Matches(FooterKey.CC));
		}

		private RevCommit Parse(string msg)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("tree " + ObjectId.ZeroId.Name + "\n");
			buf.Append("author A. U. Thor <a@example.com> 1 +0000\n");
			buf.Append("committer A. U. Thor <a@example.com> 1 +0000\n");
			buf.Append("\n");
			buf.Append(msg);
			RevWalk walk = new RevWalk(db);
			walk.SetRetainBody(true);
			RevCommit c = new RevCommit(ObjectId.ZeroId);
			c.ParseCanonical(walk, Constants.Encode(buf.ToString()));
			return c;
		}
	}
}