using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using LibraryCatalog.Models;

namespace LibraryCatalog.TestTools
{
  [TestClass]
  public class BookTests : IDisposable
  {
    public BookTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889 ;database=library_test;";
    }

    public void Dispose()
    {
      Book.DeleteAll();
      // Client.DeleteAll();
    }

    [TestMethod]
    public void GetAll_BooksEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = Book.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Save_SavesBooksToDatabase_BookList()
    {
      //Arrange
      Book testBook = new Book("White Like Me", "Tim Wise");
      testBook.Save();

      //Act
      List<Book> result = Book.GetAll();
      List<Book> testList = new List<Book>{testBook};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }
  }
}
