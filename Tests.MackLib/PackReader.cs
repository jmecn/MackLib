﻿using MackLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.MackLib
{
	public class PackReaderTest
	{
		[Fact]
		public void GetMabinogiDirectory()
		{
			Assert.False(string.IsNullOrWhiteSpace(PackReader.GetMabinogiDirectory()));
			Assert.False(!Directory.Exists(PackReader.GetMabinogiDirectory()));
		}

		[Fact]
		public void OpenReader()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				Assert.InRange(pr.Count, 1, 100000);
				Assert.InRange(pr.PackCount, 1, 1000);
			}
		}

		[Fact]
		public void OpenReaderWithInvalidPath()
		{
			Assert.Throws(typeof(ArgumentException), () =>
			{
				using (var pr = new PackReader("some/path/that/hopefully/doesn't/exist"))
				{
				}
			});
		}

		[Fact]
		public void GetData()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				var itemdb = pr.GetEntry(@"db\itemdb.xml");
				Assert.NotEqual(null, itemdb);

				var data = itemdb.GetData();
				Assert.Equal("FF-FE-3C-00-3F-00-78-00-6D-00-6C-00-20-00-76-00-65-00-72-00-73-00-69-00-6F-00-6E-00-3D-00-22-00-31-00-2E-00-30-00-22-00-20-00-65-00-6E-00-63-00-6F-00-64-00-69-00-6E-00-67-00-3D-00-22-00-75-00-74-00-66-00-2D-00-31-00-36-00-22-00-3F-00-3E-00-0D-00-0A-00-3C-00-21-00-2D-00-2D-00-20-00-44-00-4F-00-20-00-4E-00-4F-00-54-00-20-00-45-00-44-00-49-00-54-00-20-00-54-00-48-00-49-00-53-00-20-00-46-00-49-00-4C-00-45-00-21-00-20-00-2D-00-2D-00-3E-00-0D-00-0A-00-3C-00-49-00-74-00-65-00-6D-00-73-00-3E-00", BitConverter.ToString(data, 0, 164));
			}
		}

		[Fact]
		public void ReadingFileData()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				var itemdb = pr.GetEntry(@"db\itemdb.xml");
				Assert.NotEqual(null, itemdb);

				using (var sr = new StreamReader(itemdb.GetDataAsStream()))
				{
					Assert.Equal(sr.ReadLine(), "<?xml version=\"1.0\" encoding=\"utf-16\"?>");
					Assert.Equal(sr.ReadLine(), "<!-- DO NOT EDIT THIS FILE! -->");
					Assert.Equal(sr.ReadLine(), "<Items>");
				}
			}
		}

		[Fact]
		public void ReadingFileStream()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				var itemdb = pr.GetEntry(@"db\keyword.xml");
				Assert.NotEqual(null, itemdb);

				using (var sr = new StreamReader(itemdb.GetDataAsFileStream()))
				{
					Assert.Equal(sr.ReadLine(), "<?xml version=\"1.0\" encoding=\"utf-16\"?>");
					Assert.Equal(sr.ReadLine(), "<!-- DO NOT EDIT THIS FILE! -->");
					Assert.Equal(sr.ReadLine(), "<Keyword>");
				}
			}
		}

		[Fact]
		public void GetEntry()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				var entry = pr.GetEntry(@"db\race.xml");
				Assert.NotEqual(null, entry);
			}
		}

		[Fact]
		public void GetEntriesByFileName()
		{
			var path = Path.Combine(PackReader.GetMabinogiDirectory(), "package");
			using (var pr = new PackReader(path))
			{
				var entries = pr.GetEntriesByFileName("aidescdata_human.xml");
				Assert.True(entries.Count > 0);
				Assert.Single(entries, a => a.FullName == @"db\ai\local\aidescdata_human.xml");
			}
		}
	}
}
