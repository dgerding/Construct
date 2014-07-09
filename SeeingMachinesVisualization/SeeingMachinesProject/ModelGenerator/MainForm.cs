using SMFramework;
using SMFramework.Bayes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace BayesDesigner
{
    public partial class MainForm : Form
    {
        SMFramework.Bayes.ModelGenerator m_Generator = new SMFramework.Bayes.ModelGenerator();
        SMFramework.SensorClusterConfiguration m_Cluster = null;
        SMFramework.Bayes.LabeledBayes m_GeneratedBayes = null;

        public MainForm()
        {
            InitializeComponent();

            btnRemoveClass.Enabled = false;

            splitContainer.Enabled = false;
            splitContainer.Panel2.Enabled = false;
        }

        private void btnAddClass_Click(object sender, EventArgs e)
        {
            RequestStringBox rsb = new RequestStringBox();
            if (rsb.ShowDialog() != DialogResult.OK)
                return;

            if (lbClassList.Items.IndexOf(rsb.Text) != -1)
            {
                MessageBox.Show("A class with that name already exists.");
                return;
            }

            m_Generator.DataSources.Add(rsb.Text, new List<PairedDatabase>());

            lbClassList.Items.Add(rsb.Text);
            lbClassList.SetSelected(lbClassList.Items.Count - 1, true);
        }

        private void btnRemoveClass_Click(object sender, EventArgs e)
        {
            m_Generator.DataSources.Remove(lbClassList.SelectedItem as String);

            lbClassList.Items.RemoveAt(lbClassList.SelectedIndex);
        }

        private void lbClassList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbSampleList.Items.Clear();

            if (lbClassList.SelectedItem == null)
            {
                btnRemoveClass.Enabled = false;
                splitContainer.Panel2.Enabled = false;
                return;
            }

            btnRemoveClass.Enabled = true;

            splitContainer.Panel2.Enabled = true;

            foreach (PairedDatabase database in m_Generator.DataSources[lbClassList.SelectedItem as String])
            {
                lbSampleList.Items.Add(database.PersistenceFileName);
            }
        }

        private void btnSetCluster_Click(object sender, EventArgs e)
        {
            if (clusterFileDialog.ShowDialog() != DialogResult.OK)
                return;

            m_Cluster = new SMFramework.SensorClusterConfiguration(clusterFileDialog.FileName, SMFramework.SensorClusterConfiguration.FileLoadResponse.FailIfMissing);

            clbCluster.Items.Clear();
            clbCluster.Items.AddRange(m_Cluster.SensorConfigurations.ToArray());
        }

        private void clbCluster_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbCluster.CheckedItems.Count == 0)
            {
                splitContainer.Enabled = false;
                return;
            }

            splitContainer.Enabled = true;

            /* Only allow one to be checked (yes we could just use a listbox, but that wouldn't enforce the idea that only the selected
             *  signal is going to be used.)
             */
            object selected = clbCluster.SelectedItem;

            for (int i = 0; i < clbCluster.Items.Count; i++)
                clbCluster.SetItemChecked(i, false);

            clbCluster.SetItemChecked(clbCluster.Items.IndexOf(selected), true);
        }

        private void btnAddSamples_Click(object sender, EventArgs e)
        {
            if (samplesFileDialog.ShowDialog() != DialogResult.OK)
                return;

            lbSampleList.Enabled = false;
            this.Update();

            foreach (String fileName in samplesFileDialog.FileNames)
            {
                String loadingMessage = "Loading...";
                try
                {
                    lbSampleList.Items.Add(loadingMessage);
                    this.Update();

                    PairedDatabase searchedDatabase = m_Generator.DataSources[lbClassList.SelectedItem as String].Find(delegate(PairedDatabase database)
                    {
                        return database.PersistenceFileName == fileName;
                    });

                    if (searchedDatabase != null)
                        throw new Exception("A data class cannot have two of the same sample.");

                    PairedDatabase newDatabase = new PairedDatabase(fileName);
                    lbSampleList.Items.Remove(loadingMessage);
                    lbSampleList.Items.Add(fileName);
                    m_Generator.AddDataSource(newDatabase, lbClassList.SelectedItem as String);

                    this.Update();
                }
                catch (System.Exception ex)
                {
                    if (lbSampleList.Items.IndexOf(loadingMessage) != -1)
                        lbSampleList.Items.Remove(loadingMessage);

                    MessageBox.Show(String.Format("Unable to load sample {0}\nException: {1}", fileName, ex.Message));
                }
            }

            lbSampleList.Enabled = true;
        }

        private void btnRemoveSamples_Click(object sender, EventArgs e)
        {
            List<String> fileNames = new List<String>();
            foreach (String value in lbSampleList.SelectedItems)
                fileNames.Add(value);

            /* Use a proxy List<String> so that we can modify SelectedItems while iterating */
            foreach (String fileName in fileNames)
            {
                var databaseList = m_Generator.DataSources[lbClassList.SelectedItem as String];
                databaseList.Remove(databaseList.Find(delegate(PairedDatabase database)
                {
                    return database.PersistenceFileName == fileName;
                }));

                lbSampleList.Items.Remove(fileName);
            }
        }

        private void lbSampleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSampleList.SelectedIndices.Count == 0)
            {
                btnRemoveSamples.Enabled = false;
                return;
            }

            btnRemoveSamples.Enabled = true;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            /* Bunch of safety checks */

            if (m_Cluster == null || clbCluster.SelectedIndex == -1)
            {
                MessageBox.Show("A source signal must be selected for training. Within a recording may be multiple signals, and one must be selected.");
                return;
            }

            if (lbClassList.Items.Count < 2)
            {
                MessageBox.Show("A classifier must have at least 2 classes.");
                return;
            }

            List<String> emptyClasses = new List<String>();
            foreach (var pair in m_Generator.DataSources)
            {
                if (pair.Value.Count == 0)
                    emptyClasses.Add(pair.Key);
            }

            if (emptyClasses.Count > 0)
            {
                MessageBox.Show("Every class needs at least one data sample; the following classes have no samples:\n\n" + String.Join("\n", emptyClasses));
                return;
            }

            /* Do the actual calculations */

            double accuracy;
            lblGenerate.Text = "Generating model...";
            this.Update();

			try
			{
				m_Generator.ConverterSource = new SingleDirectConverter(clbCluster.SelectedItem.ToString());
				m_GeneratedBayes = m_Generator.GenerateModel(out accuracy);

				lblGenerate.Text = String.Format("Model accuracy: {0:N2}%", accuracy * 100.0);
			}
			catch (Exception ex)
			{
				lblGenerate.Text = "Error: " + ex.Message;
				return;
			}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (m_GeneratedBayes == null)
            {
                MessageBox.Show("No Bayes classifier has been generated yet.");
                return;
            }

            if (modelSaveDialog.ShowDialog() != DialogResult.OK)
                return;

            using (Stream fileStream = File.OpenWrite(modelSaveDialog.FileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, m_GeneratedBayes);
            }

			if (!cbxSaveReport.Checked)
				return;

			/* Generate confidence report */
			var dataSource = m_Generator.DataSources;


			SnapshotConverter converter = m_Generator.ConverterSource;
			foreach (var classSamplePair in dataSource)
			{
				foreach (PairedDatabase sampleData in classSamplePair.Value)
				{
					String summaryFileName = new FileInfo(modelSaveDialog.FileName).DirectoryName;
					summaryFileName = Path.Combine(summaryFileName, Path.GetFileNameWithoutExtension(modelSaveDialog.FileName) + " Probabilities (" + Path.GetFileNameWithoutExtension(sampleData.PersistenceFileName) + ").csv");

					using (StreamWriter stream = new StreamWriter(summaryFileName))
					{
						/* Write the data labels */
						stream.Write("Index,");
						stream.WriteLine(String.Join(",", m_GeneratedBayes.WordMap.CodeToString.Values));

						int index = 0;
						foreach (DataSnapshot snapshot in sampleData.PairingSnapshots)
						{
							double[] confidences = new double[converter.ClassCount];
							double unused;

							m_GeneratedBayes.Bayes.Compute(converter.GenerateFromSnapshot(snapshot), out unused, out confidences);
							stream.Write(++index + ",");
							stream.WriteLine(String.Join(",", confidences));
						}
					}
				}
			}
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            m_Generator = new SMFramework.Bayes.ModelGenerator();

            lbClassList.Items.Clear();
            lbSampleList.Items.Clear();
        }
    }
}
