using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace LibraryCatalog.Models
{
  public class Checkout
  {
    private int _id;
    private int _bookid;
    private int _clientid;
    private string _dateborrowed;
    private string _duedate;
    private int _returnstatus;

    public Checkout(int bookid, int clientid, string dateborrowed, string duedate, int returnstatus = 0, int id = 0)
    {
      _id = id;
      _bookid = bookid;
      _clientid = clientid;
      _dateborrowed = dateborrowed;
      _duedate = duedate;
      _returnstatus = returnstatus;
    }

    public int GetId()
    {
      return _id;
    }

    public int GetBookId()
    {
      return _bookid;
    }

    public int GetClientId()
    {
      return _clientid;
    }

    public string GetDateBorrowed()
    {
      return _dateborrowed;
    }

    public string GetDueDate()
    {
      return _duedate;
    }

    public int GetReturnStatus()
    {
      return _returnstatus;
    }
}
}
