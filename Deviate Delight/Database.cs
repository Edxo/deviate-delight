using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Deviate_Delight
{
    class Database
    {
        public void SetDatabase(string filename)
        {
            m_filename = filename;
        }

        public int OpenDatabase()
        {
            m_db = new SQLiteConnection(m_filename);
            m_db.Open();

            m_cmd = new SQLiteCommand(m_db);

            int res = CreateDatabaseTables();
            if(res != 0)
            {
                CloseDatabase();
                return 2;
            }
            return 0;
        }

        public void CloseDatabase()
        {
            m_db.Close();
        }

        public List<Sequences_t> ReadSequences()
        {
            m_cmd.CommandText = "SELECT id, name FROM Sequences";
            SQLiteDataReader rdr = m_cmd.ExecuteReader();

            List<Sequences_t> seqs = new List<Sequences_t>();
            while (rdr.Read())
            {
                seqs.Add(new Sequences_t(rdr.GetInt32(0), rdr.GetString(1)));
            }
            rdr.Close();
            return seqs;
        }

        public void SaveSequence(string name, Sequence_t seq)
        {
            m_cmd.CommandText = "DELETE FROM Sequences WHERE name=@name";
            m_cmd.Parameters.AddWithValue("@name", name);
            m_cmd.Prepare();
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "INSERT INTO Sequences (name) VALUES (@name)";
            m_cmd.Prepare();
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "SELECT id FROM Sequences WHERE name=@name";
            m_cmd.Prepare();
            int id = Convert.ToInt32(m_cmd.ExecuteScalar().ToString());

            m_cmd.CommandText = "INSERT INTO SequenceAction (sequence, type, value, duration, keybind) VALUES (@sequence, @type, @value, @duration, @keybind)";
            m_cmd.Parameters.AddWithValue("@sequence", id);
            foreach (var act in seq.actions)
            {
                if (act.keybind != null)
                {
                    int keybind = SaveKeyBind(act.keybind);
                    m_cmd.Parameters.AddWithValue("@keybind", keybind);
                }
                else
                    m_cmd.Parameters.AddWithValue("@keybind", null);

                m_cmd.Parameters.AddWithValue("@type", act.type);
                m_cmd.Parameters.AddWithValue("@value", act.value);
                m_cmd.Parameters.AddWithValue("@duration", act.duration);

                m_cmd.Prepare();
                m_cmd.ExecuteNonQuery();
            }
        }

        public Sequence_t LoadSequence(string name)
        {
            m_cmd.CommandText = "SELECT SA.type, SA.value, SA.duration, SA.keybind FROM Sequences S JOIN SequenceAction SA ON SA.sequence = S.id WHERE S.name=@name";
            m_cmd.Parameters.AddWithValue("@name", name);
            m_cmd.Prepare();

            SQLiteDataReader rdr = m_cmd.ExecuteReader();

            List<Action_t> actions = new List<Action_t>();
            while (rdr.Read())
            {
                Action_t act = new Action_t(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2));

                try
                {
                    int keybind = rdr.GetInt32(3);

                    Keybind_t kb = LoadKeyBind(keybind);
                    kb.readable = rdr.GetString(1);
                    act.keybind = kb;
                }
                catch (InvalidCastException) { }

                actions.Add(act);
            }

            rdr.Close();
            Sequence_t seqs = new Sequence_t(actions);
            return seqs;
        }

        public void SaveToggle(Keybind_t key)
        {
            m_cmd.CommandText = "UPDATE GenericActionKeys SET keybind=@keybind, desc=@desc WHERE name = 'toggle'";

            if (key == null)
            {
                m_cmd.Parameters.AddWithValue("@keybind", key);
                m_cmd.Parameters.AddWithValue("@desc", "N/A");
                m_cmd.Prepare();
                m_cmd.ExecuteNonQuery();
                return;
            }

            int id = SaveKeyBind(key);
            m_cmd.Parameters.AddWithValue("@keybind", id);
            m_cmd.Parameters.AddWithValue("@desc", key.readable);
            m_cmd.Prepare();
            m_cmd.ExecuteNonQuery();
        }

        public Keybind_t LoadToggle()
        {
            m_cmd.CommandText = "SELECT desc, keybind FROM GenericActionKeys WHERE name = 'toggle'";

            Keybind_t kb = null;
            SQLiteDataReader rdr = m_cmd.ExecuteReader();
            while(rdr.Read())
            {
                try
                {
                    string desc = rdr.GetString(0);
                    int keybind = rdr.GetInt32(1);

                    kb = LoadKeyBind(keybind);
                    kb.readable = desc;
                } catch{ }
            }
            rdr.Close();

            return kb;
        }

        private int SaveKeyBind(Keybind_t keybind)
        {
            Keycombo_t key = keybind.m_key;
            SQLiteCommand lcmd = new SQLiteCommand(m_db);
            try
            {
                lcmd.CommandText = "INSERT INTO ActionKey (keycode, shift, control, alt) VALUES (@keycode, @shift, @control, @alt)";
                lcmd.Parameters.AddWithValue("@keycode", (int)key.KeyCode);
                lcmd.Parameters.AddWithValue("@shift", key.Shift ? 1 : 0);
                lcmd.Parameters.AddWithValue("@control", key.Control ? 1 : 0);
                lcmd.Parameters.AddWithValue("@alt", key.Alt ? 1 : 0);
                lcmd.Prepare();
                lcmd.ExecuteNonQuery();
            }
            catch { }

            lcmd.CommandText = "SELECT id FROM ActionKey WHERE keycode=@keycode AND shift=@shift AND control=@control AND alt=@alt";
            lcmd.Prepare();
            return Convert.ToInt32(lcmd.ExecuteScalar().ToString());
        }

        private Keybind_t LoadKeyBind(int id)
        {
            SQLiteCommand lcmd = new SQLiteCommand(m_db);
            lcmd.CommandText = "SELECT keycode, shift, control, alt FROM ActionKey WHERE id=@id";
            lcmd.Parameters.AddWithValue("@id", id);
            lcmd.Prepare();
            SQLiteDataReader rdr = lcmd.ExecuteReader();

            Keybind_t kb = new Keybind_t();
            kb.m_key = new Keycombo_t();
            while (rdr.Read())
            {
                kb.m_key.KeyCode = (Keys)rdr.GetInt32(0);
                kb.m_key.Shift = rdr.GetInt32(1) == 1;
                kb.m_key.Control = rdr.GetInt32(2) == 1;
                kb.m_key.Alt = rdr.GetInt32(3) == 1;
            }
            rdr.Close();
            return kb;
        }

        private int CreateDatabaseTables()
        {
            m_cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Sequences';";
            Object name = m_cmd.ExecuteScalar();
            if (name != null)
                return 0;

            m_cmd.CommandText = "DROP TABLE IF EXISTS GenericActionKeys";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "DROP TABLE IF EXISTS SequenceAction";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "DROP TABLE IF EXISTS ActionKey";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "DROP TABLE IF EXISTS Sequences";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "CREATE TABLE Sequences (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE)";
            m_cmd.ExecuteNonQuery();
            
            m_cmd.CommandText = "CREATE TABLE ActionKey (id INTEGER PRIMARY KEY AUTOINCREMENT, keycode INTEGER, shift INTEGER, control INTEGER, alt INTEGER, UNIQUE(keycode, shift, control, alt))";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "CREATE TABLE SequenceAction (sequence INTEGER, type TEXT, value TEXT, duration INT, keybind INTEGER NULL, FOREIGN KEY(sequence) REFERENCES Sequences(id), FOREIGN KEY(keybind) REFERENCES ActionKey(id))";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "CREATE TABLE GenericActionKeys (name TEXT, desc TEXT, keybind INTEGER NULL, FOREIGN KEY(keybind) REFERENCES ActionKey(id))";
            m_cmd.ExecuteNonQuery();

            m_cmd.CommandText = "INSERT INTO GenericActionKeys (name, desc, keybind) VALUES ('toggle', 'N/A', NULL)";
            m_cmd.ExecuteNonQuery();

            return 0;
        }

        private SQLiteConnection m_db;
        private SQLiteCommand m_cmd;
        private string m_filename;
    }

    public class Sequences_t
    {
        public Sequences_t(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class Sequence_t
    {
        public Sequence_t(List<Action_t> actions)
        {
            this.actions = actions;
        }

        public List<Action_t> actions { get; set; }
    }

    public class Action_t
    {
        public Action_t(string type, string value, int duration)
        {
            this.type = type;
            this.value = value;
            this.duration = duration;
        }

        public string type { get; set; }
        public string value { get; set; }
        public int duration { get; set; }
        public Keybind_t keybind { get; set; }
    }

    public class Keybind_t
    {
        public string readable { get; set; }
        public Keycombo_t m_key { get; set; }

        public override string ToString()
        {
            return readable;
        }
    }

    public class Keycombo_t
    {
        public Keycombo_t() {
            Shift = false;
            Control = false;
            Alt = false;
            KeyCode = 0;
        }

        public Keycombo_t(Keys code)
        {
            Shift = false;
            Control = false;
            Alt = false;
            KeyCode = code;
        }

        public Keycombo_t(KeyEventArgs KeyEvent)
        {
            Shift = KeyEvent.Shift;
            Control = KeyEvent.Control;
            Alt = KeyEvent.Alt;
            KeyCode = KeyEvent.KeyCode;
        }


        public bool Shift { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public Keys KeyCode { get; set; }
    }
}
