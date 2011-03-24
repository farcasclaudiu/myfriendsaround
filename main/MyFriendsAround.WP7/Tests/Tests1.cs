using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if TESTING

namespace MyFriendsAround.WP7.Tests
{
    [TestClass]
    public class Tests1
    {
        [TestMethod]
        public void Test_If_This_One_Is_Correct()
        {
            Assert.IsTrue(true, "this method always pass");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Must_Throw_Error()
        {
            //
        }
    }
}

#endif