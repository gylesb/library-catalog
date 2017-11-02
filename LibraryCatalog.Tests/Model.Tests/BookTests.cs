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

    [TestMethod]
    public void Save_DatabaseAssignsIdToTitle_Id()
    {
      //Arrange
      Book testBook = new Book("White Like Me", "Tim Wise");
      testBook.Save();

      //Act
      Book savedBook = Book.GetAll()[0];

      int result = savedBook.GetId();
      int testId = testBook.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Find_FindsBookInDatabase_Book()
    {
      //Arrange
      Book testBook = new Book("White Like Me", "Tim Wise");
      testBook.Save();

      //Act
      Book foundBook = Book.Find(testBook.GetId());

      //Assert
      Assert.AreEqual(testBook, foundBook);
    }

    [TestMethod]
    public void Delete_DeletesCourseAssociationsFromDatabase_BookList()
    {
      //Arrange
      Copy testCopy = new Copy(6, 8);
      testCopy.Save();

      Book testBook = new Book("White Like Me", "Tim Wise");
      testBook.Save();

      //Act
      testBook.AddCopy(testCopy);
      testBook.Delete();

      List<Book> resultCopyBooks = testCopy.GetBook();
      List<Book> testCopyBooks = new List<Book> {};

      //Assert
      CollectionAssert.AreEqual(testCopyBooks, resultCopyBooks);
    }
  }
}
