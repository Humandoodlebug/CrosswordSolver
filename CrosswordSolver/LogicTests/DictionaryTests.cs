﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.CrosswordSolver.Logic;

namespace Logic.Tests
{
    [TestClass]
    public class DictionaryTests
    {
        [TestMethod]
        public void VerifyTest()
        {
            var dict = new Dictionary("Dictionaries/en_GB.aff", "Dictionaries/en_GB.dic");
            Assert.IsTrue(dict.Verify("it"));
            Assert.IsTrue(dict.Verify("dictionary"));
            Assert.IsFalse(dict.Verify("dictionary"));
        }

        [TestMethod]
        public void StemTest()
        {
            var dict = new Dictionary("Dictionaries/en_GB.aff", "Dictionaries/en_GB.dic");
            Assert.AreEqual("jump", dict.Stem("jumping")[0]);
        }

        [TestMethod()]
        public void GenerateTest()
        {
            var dict = new Dictionary("Dictionaries/en_GB.aff", "Dictionaries/en_GB.dic");
            CollectionAssert.Contains(dict.Generate("girl", "boys"), "girls");
        }
    }
}