using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet;
using System.Management;

namespace Mando_Ps4_PC
{
    public partial class Form1 : Form
    {
        private String rutaInputMapper;
        private List<String> programasListaNegra;
        private List<String> programasACerrar;
        private bool primerClick = true;

        public Form1()
        {
            
            programasListaNegra = new List<string>();
            InitializeComponent();
            //lecturaSettings();
            //inicializarListaNegra();
            mandoConectat();
        }
        private void buscarInputMapper()
        {
            if (DialogResult.OK == MessageBox.Show("Por favor, localize el archivo InputMapper.exe o DS4Windows.exe en su equipo."))
            {
                String directori_inicial = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "DSDCS", "DS4Windows");
                if (!Directory.Exists(directori_inicial))
                    directori_inicial = "C:\\";

                openFileDialog1.InitialDirectory = directori_inicial;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    rutaInputMapper = openFileDialog1.FileName;
                    Properties.Settings.Default.Ruta_InputMapper = rutaInputMapper;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void lecturaSettings()
        {
            if (Properties.Settings.Default.Preguntar_Cerrar)
            {
                menuPreguntarCerrar.Checked = true;
            }
            else
            {
                menuPreguntarCerrar.Checked = false;
            }

            if (Properties.Settings.Default.Ruta_InputMapper != "")
            {
                rutaInputMapper = Properties.Settings.Default.Ruta_InputMapper;
            }
            else
            {
                buscarInputMapper();
            }
        }

        private void inicializarListaNegra()
        {
            programasListaNegra.Add("explorer");
            programasListaNegra.Add("Steam");
            programasListaNegra.Add("ts3client_win64");
            programasListaNegra.Add("ts3client_win32");
        }

        private bool cerrarProgramasListaNegra()
        {
            
            StringBuilder str = new StringBuilder();
            programasACerrar = new List<string>();

            foreach(String program in programasListaNegra)
            {
                Process[] instancias = Process.GetProcessesByName(program);
                Console.WriteLine(program);
                if (instancias.Length > 0)
                {
                    programasACerrar.Add(program);
                    str.Append(", " + program);
                }
                
                
            }
            if (programasACerrar.Count == 0)
                return true;

            DialogResult mes = DialogResult.None;
            if (menuPreguntarCerrar.Checked)
            {
                mes = MessageBox.Show(String.Format("¿Quiere cerrar los programas {0}?", str.ToString()),"Cerrar Programas Conflictivos", MessageBoxButtons.OKCancel);
            }
            if(!menuPreguntarCerrar.Checked || mes == DialogResult.OK)
            {
                Console.WriteLine("Programes tancats: ------");
                foreach (String program in programasACerrar)
                {
                    Process[] instancias = Process.GetProcessesByName(program);
                    foreach(Process p in instancias)
                    {
                        try {
                            Console.WriteLine(p.ProcessName);
                            ForceKill(p);
                        }catch(Exception e)
                        {
                            Console.WriteLine("programa no tancat");
                            return false;
                        }
                    }
                }
            }else if(mes == DialogResult.Cancel)
            {
                return false;
            }
            return true;
        }
        
        private void obreProgramesTancats()
        {

            Process.Start(Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));

        }

        private void obreInputMapper()
        {
            ProcessStartInfo p = new ProcessStartInfo("notepad.exe");
            p.WindowStyle = ProcessWindowStyle.Minimized;
            Process.Start(p);
        }

        public bool IsUsbDeviceConnected(string pid, string vid)
        {
            using (var searcher =
              new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
            {
                using (var collection = searcher.Get())
                {
                    foreach (var device in collection)
                    {
                        
                        var usbDevice = Convert.ToString(device);
                        Console.WriteLine(usbDevice);
                        if (usbDevice.Contains(pid) && usbDevice.Contains(vid))
                        {
                            Console.WriteLine("DeviceName:"+device.GetPropertyValue("Name"));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool mandoConectat()
        {
            return IsUsbDeviceConnected("028", "045");
        }

        private void btPrincipal_Click(object sender, EventArgs e)
        {
            if (mandoConectat())
                lbInfo.Text="mando conectat";
            else
                lbInfo.Text = "mando NO conectat";
            
            /*if (primerClick)
            {   //fase tancar
                primerClick = false;
                btPrincipal.Text = "Cancelar";
                Boolean cerrados = cerrarProgramasListaNegra();
                if (!cerrados)
                {
                    while (!cerrados && MessageBox.Show("No se pudieron cerrar los porgramas necesarios. Si no es la primera vez que ve este mensaje trate de ejecutar el programa como Administrador\n ¿Reintentar?", "¿Reintentar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cerrados = cerrarProgramasListaNegra();
                    }
                }
                //fase obir InputMapper
                //obreInputMapper();

                //fase conecta mando

                //fase obrir explorer
                obreProgramesTancats();
            }
            else
            {
                //fase Tornar a obrir
                primerClick = true;
                btPrincipal.Text = "Empezar";
                obreProgramesTancats();
            }*/


        }

        private void menuPreguntarCerrar_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Preguntar_Cerrar = menuPreguntarCerrar.Checked;
            Properties.Settings.Default.Save();
        }

        private void menuUbicacion_Click(object sender, EventArgs e)
        {
            buscarInputMapper();
        }

        public void ForceKill(Process process)
        {
            using (Process killer = new Process())
            {
                killer.StartInfo.FileName = "taskkill";
                killer.StartInfo.Arguments = string.Format("/f /PID {0}", process.Id);
                killer.StartInfo.CreateNoWindow = true;
                killer.StartInfo.UseShellExecute = false;
                killer.Start();
                killer.WaitForExit();
                if (killer.ExitCode != 0)
                {
                    throw new Win32Exception(killer.ExitCode);
                }
            }
        }
    }
}
