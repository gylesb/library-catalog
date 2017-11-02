using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace LibraryCatalog.Models
{
  public class Copy
  {
    private int _id;
    private int _copies;

    public Copy(int copies, int Id = 0)
    {
      _copies = copies;
      _id = Id;
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public int GetCopies()
    {
      return _copies;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Copy> GetAll()
    {
      List<Copy> allCopies = new List<Copy> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM copies;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int CopyId = rdr.GetInt32(0);
        int CopyNumber = rdr.GetInt32(1);
        Copy newCopy = new Copy(CopyNumber, CopyId);
        allCopies.Add(newCopy);
      }
      conn.Close();

      if(conn != null)
      {
        conn.Dispose();
      }

      return allCopies;
    }

    public void AddCopy(Book newBook)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE copies SET copies += 1 WHERE bookId = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = newBook.GetId();
      cmd.Parameters.Add(searchId);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void RemoveCopy(Book newBook)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE copies SET copies -= 1 where bookId = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = newBook.GetId();
      cmd.Parameters.Add(searchId);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
