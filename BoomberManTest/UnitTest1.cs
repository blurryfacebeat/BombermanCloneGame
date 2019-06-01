using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Boomberman;

namespace BoomberManTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void boxSizeCreate()
        {
            // Arrange

            // Act
            Bomb c = new Bomb();

            int actual = c.PreparePercent();

            // Assert
        }
    }
}
