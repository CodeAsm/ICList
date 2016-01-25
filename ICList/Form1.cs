using ICList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICList
{

    public partial class Form1 : Form
    {
        //   private int selectedIC;
        // Declare a new BindingListOfT with the Part business object.
        BindingList<IC> ICList;
        private void InitializeListOfParts()
        {
            // Create the new BindingList of Part type.
            ICList = new BindingList<IC>();

            // Allow new parts to be added, but not removed once committed.        
            ICList.AllowNew = false;
            ICList.AllowRemove = false;

            // Raise ListChanged events when new parts are added.
            ICList.RaiseListChangedEvents = true;

            // Do not allow parts to be edited.
            ICList.AllowEdit = false;

            // Add a couple of parts to the list.
            // listOfParts.Add(new Part("Widget", 1234));
            //listOfParts.Add(new Part("Gadget", 5647));
        }



        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Read sample data from CSV file
                using (CsvFileReader reader = new CsvFileReader(txtFile.Text))
                {
                    CsvRow row = new CsvRow();
                    while (reader.ReadRow(row))
                    {
                        ICList.Add(new IC(row[0], Convert.ToInt32(row[2]), row[1], row[3]));
                    }
                }
                ICViewupdate("");

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Meh, file not found. or something else.");
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Format foobar.");
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text != "" && txtNumber.Text != "") {
                
                addIC(txtName.Text, txtDateCode.Text, Convert.ToInt32(txtNumber.Text));
            }
        }

        private void addIC(string Name, string DateCode, int Number)
        {
            string PakageType = "NULL"; //TODO: Pakage type definitions, as also locations
            IC newIC = new IC(Name,Number,DateCode, PakageType);
           
            ICList.Add(newIC);
            
            ICViewupdate(newIC.name);

        }

        private void ICViewupdate(string name)
        {
            if (name == "" && ICList.Count >= 1)
            {
                IC bob = ICList.First();
                name = bob.name;
            }
            var item = ICList.FirstOrDefault(i => i.name == name);
         
            if (item != null )
            {
                txtName.Text = item.name;
                txtNumber.Text = item.number.ToString();
                txtDateCode.Text = item.datecode;
                lblTotal.Text = "Total IC in DB: " + ICList.Count();
            }
            if (item == null) {
                clearGui();
            }
            else
            {

                listBox1.DataSource = ICList;
            }
            listBox1.Update();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ICList.Clear();
           // ICViewupdate("");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
    // var enumerator = ICList.GetEnumerator();
    // if (!enumerator.MoveNext())
    //     yield break;
    //
    //           while (true)
    //         {
    //            yield return enumerator.Current;
    //             if (!enumerator.MoveNext())
    //               enumerator = ICList.GetEnumerator();
    //        }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            InitializeListOfParts();
            listBox1.DataSource = ICList;
            listBox1.DisplayMember = "name";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TODO: Fix after clean, this breaks cause no ICs anymore
            if (ICList.Count >= 1)
            {
                IC bob = ICList.ElementAt(listBox1.SelectedIndex);
                ICViewupdate(bob.name);
            }
            else {
                clearGui();
            }
        }

        private void clearGui()
        {
            txtName.Text = "";
            txtNumber.Text = "";
            txtDateCode.Text = "";
            lblTotal.Text = "Total IC in DB: 0";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            IC bob = new IC(txtName.Text, Convert.ToInt32(txtNumber.Text), txtDateCode.Text, "");
            ICList.Remove(bob);

        }
    }



    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }

}
