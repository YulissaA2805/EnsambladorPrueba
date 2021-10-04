﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EnsambladorPrueba
{
    class Program
    {
        class Instruccion
        {

            public int Tamano { get; set; }

            public int Codigo { get; set; }

            public override string ToString()
            {
                return "Tamaño: " + Tamano + "   Codigo de Operacion: " + Codigo;
            }
        }

        class Variable
        {
            public string Nombre { get; set; }

            public int Direccion { get; set; }

            public string Tipo { get; set; }

            public string numeroElementos { get; set; }

            public string vectorString { get; set; }

            public override string ToString()
            {
                return "Nombre: " + Nombre + "   Direccion: " + Direccion + "   Tipo: " + Tipo + "   Numero de elementos: " + numeroElementos + "   VS: " + vectorString;
            }
        }

        private static Dictionary <string, Instruccion> instrucciones = new Dictionary<string, Instruccion>()
        {
            { "NOP", new Instruccion{ Tamano=1, Codigo=0 } },
            { "DEFI", new Instruccion{ Tamano=0, Codigo=-1 } },
            { "DEFD", new Instruccion{ Tamano=0, Codigo=-1 } },
            { "DEFS",new Instruccion{ Tamano=0, Codigo=-1 } },
            { "DEFAI", new Instruccion{ Tamano=0, Codigo=-1 } },
            { "DEFAD",new Instruccion{ Tamano=0, Codigo=-1 } },
            { "DEFAS", new Instruccion{ Tamano=0, Codigo=-1 } },

            { "ADD", new Instruccion{ Tamano=1, Codigo=1 } },
            { "SUB", new Instruccion{ Tamano=1, Codigo=2 } },
            { "MULT", new Instruccion{ Tamano=1, Codigo=3 } },
            { "DIV", new Instruccion{ Tamano=1, Codigo=4 } },
            { "MOD", new Instruccion{ Tamano=1, Codigo=5 } },
            { "INC", new Instruccion{ Tamano=3, Codigo=6 } },
            { "DEC", new Instruccion{ Tamano=3, Codigo=7 } },

            { "CMPEQ", new Instruccion{ Tamano=1, Codigo=8 }},
            { "CMPNE",new Instruccion{ Tamano=1, Codigo=9 } },
            { "CMPLT", new Instruccion{ Tamano=1, Codigo=10 } },
            { "CMPLE", new Instruccion{ Tamano=1, Codigo=11 }},
            { "CMPGT", new Instruccion{ Tamano=1, Codigo=12 } },
            { "CMPGE", new Instruccion{ Tamano=1, Codigo=13 } },

            { "JMP", new Instruccion{ Tamano=3, Codigo=14 } },
            { "JMPT",new Instruccion{ Tamano=3, Codigo=15 } },
            { "JMPF", new Instruccion{ Tamano=3, Codigo=16 } },
            { "SETIDX", new Instruccion{ Tamano=3, Codigo=17 } },
            { "SETIDXK", new Instruccion{ Tamano=5, Codigo=18 } },

            { "PUSHI", new Instruccion{ Tamano=3, Codigo=19 } },
            { "PUSHD", new Instruccion{ Tamano=3, Codigo=20}  },
            { "PUSHS", new Instruccion{ Tamano=3, Codigo=21}  },
            { "PUSHAI", new Instruccion{ Tamano=3, Codigo=22 } },
            { "PUSHAD", new Instruccion{ Tamano=3, Codigo=23}  },
            { "PUSHAS", new Instruccion{ Tamano=3, Codigo=24}  },
            { "PUSHKI",new Instruccion{ Tamano=5, Codigo=25} },
            { "PUSHKD", new Instruccion{ Tamano=9, Codigo=26 }  },
            { "PUSHKS", new Instruccion{ Tamano=2, Codigo=27}  }, //n+2

            { "POPI", new Instruccion{ Tamano=3, Codigo=28 }  },
            { "POPD", new Instruccion{ Tamano=3, Codigo=29}  },
            { "POPS", new Instruccion{ Tamano=3, Codigo=30}  },
            { "POPAI", new Instruccion{ Tamano=3, Codigo=31 }  },
            { "POPAD", new Instruccion{ Tamano=3, Codigo=32}  },
            { "POPAS", new Instruccion{ Tamano=3, Codigo=33} },
            { "POPIDX", new Instruccion{ Tamano=1, Codigo=34 }  },

            { "READI", new Instruccion{ Tamano=3, Codigo=35}  },
            { "READD", new Instruccion{ Tamano=3, Codigo=36}  },
            { "READS", new Instruccion{ Tamano=3, Codigo=37}  },
            { "READAI", new Instruccion{ Tamano=3, Codigo=38 } },
            { "READAD", new Instruccion{ Tamano=3, Codigo=39}  },
            { "READAS", new Instruccion{ Tamano=3, Codigo=40}  },

            { "PRTM", new Instruccion{ Tamano=2, Codigo=41 }  }, //n+2
            { "PRTI", new Instruccion{ Tamano=3, Codigo=42}  },
            { "PRTD", new Instruccion{ Tamano=3, Codigo=43}  },
            { "PRTS", new Instruccion{ Tamano=3, Codigo=44}  },
            { "PRTAI", new Instruccion{ Tamano=3, Codigo=45 }  },
            { "PRTAD", new Instruccion{ Tamano=3, Codigo=46}  },
            { "PRTAS", new Instruccion{ Tamano=3, Codigo=47}  },

            { "HALT", new Instruccion{ Tamano=1, Codigo=48 }  },
        };
        private static Dictionary<int, string> tipoVariable = new Dictionary<int, string>()
        {
            {0, "int"},
            {1, "double"},
            {2, "string"},
            {10, "array int"},
            {11, "array double"},
            {12, "array string"},
        };
        private static List<Variable> tabla_var = new List<Variable>();

        private static Dictionary<string, int> variables = new Dictionary<string, int>();

        private static Dictionary<string, int> etiquetas_def = new Dictionary<string, int>();

        private static Dictionary<int, string> etiquetas_refer = new Dictionary<int, string>();
        //en etiquetas_ref la llave es la dir porque se pueden repetir las etiquetas
        static void Main(string[] args)
        {
            string path = @"C:/Users/ludmi/Downloads/prueba-texto-2-en-ase.ASE";//La ruta cambia dependiendo de la computadora
            //ruta 1:@"C:/Users/93764/Desktop/pruebas bin/prueba texto 2 en ase.ASE"
            //ruta 2:@"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/prueba texto 2 en ase.ASE"
            //ruta 3:@"C:/Users/ludmi/Downloads/prueba-texto-2-en-ase.ASE"

            string result = Path.GetFileName(path);
            if(Path.GetExtension(path) != ".ASE")
            {
                throw new Exception($"La extensión '{Path.GetExtension(path)}' es incorrecta. Solo se aceptan archivos en .ASE");
                //Console.WriteLine("La extensión '{0}' es incorrecta. Solo se aceptan archivos en .ASE", Path.GetExtension(path));
                
            }
            else
            {
                Console.WriteLine("Nombre del archivo: '{0}'", result);
            }
            

            Encoding ascii = Encoding.ASCII;

            byte[] readText = File.ReadAllBytes(path);

            string a = ascii.GetString(readText);

            //Console.WriteLine("Contenido del archivo:\n" + a);

            //foreach (byte s in readText)
            //{
            //    Console.Write(s + " ");
            //}
            //Console.WriteLine();

            var lineas = a.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            int tam_seg_cod = 0;//tamaño segmento de código, primeros 12 bytes reservados
            int tam_seg_dat = 0;//tamaño segmento de datos
            int tam_vs = 0;//tamaño vector string
            int dir = 0;

            //StreamWriter sw;
            //sw.Write()

            BinaryWriter bw;

            bw = new BinaryWriter(new FileStream("C:/Users/ludmi/Downloads/probando stn.STN", FileMode.Create));
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stn.STN"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stn.STN"
            //ruta destino 3:"C:/Users/ludmi/Downloads/probando stn.STN"

            string magic_number = "ICCSTN      ";//los espacios son para dejar vacíos por ahora los segmentos y el vector string
            tam_seg_cod += 12;
            byte[] bytes = Encoding.ASCII.GetBytes(magic_number);//se escribe el magic number
            bw.Write(bytes);
            foreach(byte b in bytes)
            {
                Console.WriteLine(b + "\t"+ ((char)b).ToString());
            }

            for (var i = 0; i < lineas.Length; i++)//recorre cada línea
            {
                var numLinea = i + 1;
                var linea = lineas[i];

                var palabras_linea = linea.Split(' ');
                for(var j = 0; j <palabras_linea.Length; j++)//recorre cada palabra en la linea
                {
                    switch (palabras_linea[j])
                    {
                        case "NOP":
                            bw.Write((byte)instrucciones["NOP"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "DEFI":
                            variables.Add(palabras_linea[1], tam_seg_dat);//la siguiente palabra debe ser una variable
                            tabla_var.Add(new Variable() { Nombre = palabras_linea[1], Direccion = tam_seg_dat, Tipo = tipoVariable[0], numeroElementos = "0", vectorString = "X" });

                            tam_seg_dat += 4;//una variable int ocupa 4 bytes
                            
                            break;
                        case "DEFD":
                            variables.Add(palabras_linea[1], tam_seg_dat);//la siguiente palabra debe ser una variable
                            tabla_var.Add(new Variable() { Nombre = palabras_linea[1], Direccion = tam_seg_dat, Tipo = tipoVariable[1], numeroElementos = "0", vectorString = "X" });

                            tam_seg_dat += 8;//una variable double ocupa 8 bytes

                            break;
                        case "DEFS":
                            variables.Add(palabras_linea[1], tam_seg_dat);//la siguiente palabra debe ser una variable
                            tabla_var.Add(new Variable() { Nombre = palabras_linea[1], Direccion = tam_seg_dat, Tipo = tipoVariable[2], numeroElementos = "0", vectorString = tam_vs.ToString() });

                            tam_vs += 1;
                            tam_seg_dat += 2;//una variable string ocupa 2 bytes

                            break;
                        case "DEFAI":
                            var variables_defai = palabras_linea[1].Split(',');//se divide en nombre y #
                            variables.Add(variables_defai[0], tam_seg_dat);
                            int numero = Int32.Parse(variables_defai[1]);

                            tabla_var.Add(new Variable() { Nombre = variables_defai[0], Direccion = tam_seg_dat, Tipo = tipoVariable[10], numeroElementos = numero.ToString(), vectorString = "X" });

                            for (var k=0; k<numero; k++)
                            {
                                tam_seg_dat += 4;
                            }
                            
                            break;
                        case "DEFAD":
                            variables_defai = palabras_linea[1].Split(',');//se divide en nombre y #
                            variables.Add(variables_defai[0], tam_seg_dat);
                            numero = Int32.Parse(variables_defai[1]);

                            tabla_var.Add(new Variable() { Nombre = variables_defai[0], Direccion = tam_seg_dat, Tipo = tipoVariable[11], numeroElementos = numero.ToString(), vectorString = "X" });

                            for (var k = 0; k < numero; k++)
                            {
                                tam_seg_dat += 8;
                            }

                            break;
                        case "DEFAS":
                            variables_defai = palabras_linea[1].Split(',');//se divide en nombre y #
                            variables.Add(variables_defai[0], tam_seg_dat);
                            numero = Int32.Parse(variables_defai[1]);

                            tabla_var.Add(new Variable() { Nombre = variables_defai[0], Direccion = tam_seg_dat, Tipo = tipoVariable[12], numeroElementos = numero.ToString(), vectorString = tam_vs.ToString() });

                            tam_vs += numero;
                            for (var k = 0; k < numero; k++)
                            {
                                tam_seg_dat += 2;
                            }

                            break;
                        case "ADD":
                            bw.Write((byte)instrucciones["ADD"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "SUB":
                            bw.Write((byte)instrucciones["SUB"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "MULT":
                            bw.Write((byte)instrucciones["MULT"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "DIV":
                            bw.Write((byte)instrucciones["DIV"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "MOD":
                            bw.Write((byte)instrucciones["MOD"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "INC":
                            if (variables.ContainsKey(palabras_linea[j+1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["INC"].Codigo);//+1 byte
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//+2 bytes
                                tam_seg_cod += 3;//3 bytes
                                dir += 3;//3 bytes
                            }
                            break;
                        case "DEC":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["DEC"].Codigo);//+1 byte
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//+2 bytes
                                tam_seg_cod += 3;//3 bytes
                                dir += 3;//3 bytes
                            }
                            break;
                        case "CMPEQ":
                            bw.Write((byte)instrucciones["CMPEQ"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "CMPNE":
                            bw.Write((byte)instrucciones["CMPNE"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "CMPLT":
                            bw.Write((byte)instrucciones["CMPLT"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "CMPLE":
                            bw.Write((byte)instrucciones["CMPLE"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "CMPGT":
                            bw.Write((byte)instrucciones["CMPGT"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "CMPGE":
                            bw.Write((byte)instrucciones["CMPGE"].Codigo);//1 byte
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "JMP"://
                            if (etiquetas_def.ContainsKey(palabras_linea[j + 1]))//si es una etiqueta definida
                            {
                                bw.Write((byte)instrucciones["JMP"].Codigo);
                                bw.Write((ushort)etiquetas_def[palabras_linea[j + 1]]);//en STN se guarda dir de la etiqueta definida en 2 bytes
                                etiquetas_refer.Add(tam_seg_cod, palabras_linea[j + 1]);
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            
                            break;
                        case "JMPT":
                            if (etiquetas_def.ContainsKey(palabras_linea[j + 1]))//si es una etiqueta definida
                            {
                                bw.Write((byte)instrucciones["JMPT"].Codigo);
                                bw.Write((ushort)etiquetas_def[palabras_linea[j + 1]]);//en TSN se guarda dir de la etiqueta definida en 2 bytes
                                etiquetas_refer.Add(tam_seg_cod, palabras_linea[j + 1]);
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "JMPF"://
                            if (etiquetas_def.ContainsKey(palabras_linea[j + 1]))//si es una etiqueta definida
                            {
                                bw.Write((byte)instrucciones["JMPF"].Codigo);
                                bw.Write((ushort)etiquetas_def[palabras_linea[j + 1]]);//en TSN se guarda dir de la etiqueta definida en 2 bytes
                                etiquetas_refer.Add(tam_seg_cod, palabras_linea[j + 1]);
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "SETIDX"://
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["SETIDX"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda dirección de la variable en 2 byes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "SETIDXK"://
                            bw.Write((byte)instrucciones["SETIDXK"].Codigo);
                            tam_seg_cod += 5;
                            dir += 5;
                            bw.Write(Int32.Parse(palabras_linea[j + 1]));//guarda dirección de la variable en 4 bytes
                            break;
                        case "PUSHI":
                            if (variables.ContainsKey(palabras_linea[j+1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j+1]]);
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHAI":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHAI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHAD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHAD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHAS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PUSHAS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PUSHKI":
                            bw.Write((byte)instrucciones["PUSHKI"].Codigo);
                            bw.Write(Int32.Parse(palabras_linea[j + 1]));
                            tam_seg_cod += 5;
                            dir += 5;
                            break;
                        case "PUSHKD"://
                            bw.Write((byte)instrucciones["PUSHKD"].Codigo);
                            bw.Write(Int64.Parse(palabras_linea[j + 1]));//escribe la constante double en 8 bytes
                            tam_seg_cod += 9;
                            dir += 9;
                            break;
                        case "PUSHKS"://
                            bw.Write((byte)instrucciones["PUSHKS"].Codigo);
                            bw.Write(' ');//+1 byte
                            tam_seg_cod += 2;
                            dir += 2;
                            foreach (char c in palabras_linea[j + 1]) //+n bytes
                            {
                                bw.Write((byte)c);
                                tam_seg_cod++;
                                dir++;
                            }
                            break;
                        case "POPI":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            
                            break;
                        case "POPD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "POPS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "POPAI":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPAI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "POPAD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPAD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "POPAS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["POPAS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }

                            break;
                        case "POPIDX":
                            bw.Write((byte)instrucciones["POPIDX"].Codigo);
                            tam_seg_cod++;
                            dir++;
                            break;
                        case "READI":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "READD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "READS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "READAI":
                            if (variables.ContainsKey(palabras_linea[j+1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READAI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "READAD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READAD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "READAS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["READAS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTM"://
                            bw.Write((byte)instrucciones["PRTM"].Codigo);
                            bw.Write(' ');//+1 byte
                            tam_seg_cod += 2;
                            dir += 2;
                            foreach (char c in palabras_linea[j + 1]) //+n bytes
                            {
                                bw.Write((byte)c);
                                tam_seg_cod++;
                                dir++;
                            }
                            break;
                        case "PRTI":
                            if (variables.ContainsKey(palabras_linea[j+1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j+1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTAI":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTAI"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTAD":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTAD"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "PRTAS":
                            if (variables.ContainsKey(palabras_linea[j + 1]))//si ya se definió la variable
                            {
                                bw.Write((byte)instrucciones["PRTAS"].Codigo);
                                bw.Write((ushort)variables[palabras_linea[j + 1]]);//guarda la dirección de la variable en 2 bytes
                                tam_seg_cod += 3;
                                dir += 3;
                            }
                            break;
                        case "HALT":
                            bw.Write((byte)instrucciones["HALT"].Codigo);
                            tam_seg_cod++;
                            dir++;
                            break;
                        default:
                            if (palabras_linea[j].StartsWith(';'))//es un comentario
                            {
                                //no hace nada por el momento, nada más se salta el comentario
                                j = palabras_linea.Length;
                            }
                            else if (palabras_linea[j].Contains(':') && !etiquetas_def.ContainsKey(palabras_linea[j]))
                            {//es una etiqueta que todavía no ha sido definida
                                string nueva_etiqueta = palabras_linea[j].Trim(':');
                                etiquetas_def.Add(nueva_etiqueta, dir);
                            }
                            break;

                    }//switch palabras_linea
                }//for cada palabra en la linea
            }//for cada linea

            bw.Seek(6, SeekOrigin.Begin);//busca Seg Cod
            bw.Write((ushort)dir);//escribe en Seg Cod el peso de la sección con las instrucciones (dir)

            bw.Seek(8, SeekOrigin.Begin);//busca Seg Datos
            bw.Write((ushort)tam_seg_dat);//escribe en Seg Datos

            bw.Seek(10, SeekOrigin.Begin);//busca Vector String
            bw.Write((ushort)tam_vs);//escribe en Vector String

            bw.Close();

            string path2 = "C:/Users/ludmi/Downloads/probando stn.STN";
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stn.STN"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stn.STN"
            //ruta destino 3:"C:/Users/ludmi/Downloads/probando stn.STN"
            byte[] readText2 = File.ReadAllBytes(path2);
            //string a2 = ascii.GetString(readText2);
            //Console.WriteLine("Contenido del archivo STN:\n" + a2);
            foreach (byte s in readText2)//contenido del STN en bytes
            {
                Console.Write(s + " ");
            }
            Console.WriteLine();

            //aqui empieza el STNV
            char[] nombreV = new char[30];
            char[] copiaNV = new char[30];
            for(int iV2 = 0; iV2 < 30; iV2++)
            {
                copiaNV[iV2] = '0';
            }
            short dirV, numElementos, VS;
            byte tipoV;
            string acum = System.String.Empty;

            Console.WriteLine("Tabla de variables: ");
            foreach(Variable unaVariable in tabla_var)
            {
                nombreV = unaVariable.Nombre.ToCharArray();
                for(int iV = 0; iV<nombreV.Length; iV++)
                {
                    copiaNV[iV] = nombreV[iV];
                }
                Console.Write(copiaNV);
                for (int iV3 = 0; iV3 < copiaNV.Length; iV3++)
                {
                    acum += copiaNV[iV3];
                }
                for (int iV2 = 0; iV2 < 30; iV2++)
                {
                    copiaNV[iV2] = '0';
                }
                dirV = Convert.ToInt16(unaVariable.Direccion);
                Console.Write(dirV);
                acum += dirV;
                switch (unaVariable.Tipo)
                {
                    case "int":
                        tipoV = 0;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    case "double":
                        tipoV = 1;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    case "string":
                        tipoV = 2;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    case "array int":
                        tipoV = 10;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    case "array double":
                        tipoV = 11;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    case "array string":
                        tipoV = 12;
                        Console.Write(tipoV);
                        acum += tipoV;
                        break;
                    default:
                        Console.Write("Tipo de variable desconocido");
                        break;
                }
                numElementos = Convert.ToInt16(unaVariable.numeroElementos);
                Console.Write(numElementos);
                acum += numElementos;
                if (unaVariable.vectorString == "X")
                {
                    VS = 0;
                    Console.Write(VS);
                    acum += VS;
                }
                else
                {
                    VS = Convert.ToInt16(unaVariable.vectorString);
                    Console.Write(VS);
                    acum += VS;
                }
            }
            string path3 = "C:/Users/ludmi/Downloads/probando stnv.STNV";
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stnv.STNV"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stn.STN"
            //ruta destino 3:"C:/Users/ludmi/Downloads/probando stnv.STNV"
            File.WriteAllText(path3, acum);
            Console.WriteLine("");

            //aquí empieza desensamblador
        
            byte[] segcod = new byte[tam_seg_cod];//crea array del tamano del segmento de codigo
            int contador = 0;
            foreach (byte s in readText2)//llena el array con los bytes del segmento de codigo en cada espacio
            {
                segcod[contador] = s;
                contador++;
            }

            //lista de variables del .stnv
            List<Variable> varias = new List<Variable>();
            string varia = "";
            int varContador = 0, cont = 0;
            foreach (char c in File.ReadAllText(path3))
            {
                if(varContador < 30)//hasta 30 bytes
                {
                    if (Char.IsLetter(c))
                    {
                        varia += c;
                    }
                    varContador++;
                }
                else if(Char.IsLetter(c))//verifica si hay otra variables
                {
                    //se añade la variable guardada en varia a la lista
                    varias.Add(new Variable() { Nombre = varia, Direccion = tabla_var[cont].Direccion, Tipo = "", numeroElementos = "", vectorString = "" });
                    cont++;
                    varia = "";
                    varia += c;
                    varContador = 1;
                }
            }

            varias.Add(new Variable() { Nombre = varia, Direccion = tabla_var[cont].Direccion, Tipo = "", numeroElementos = "", vectorString = "" });


            int conta;
            for (contador = 12; contador < tam_seg_cod; contador++)//recorre el array, compara con el codigo de cada instruccion
            {                                                     //y escribe la instruccion que corresponde
                conta = 0;
                switch (segcod[contador])//Imprime la instruccion segun el codigo de operacion, y aumenta el espacio recorrido segun la instruccion
                {
                    case 1:
                        Console.WriteLine("ADD");
                        break;
                    case 2:
                        Console.WriteLine("SUB");
                        break;
                    case 3:
                        Console.WriteLine("MULT");
                        break;
                    case 4:
                        Console.WriteLine("DIV");
                        break;
                    case 5:
                        Console.WriteLine("MOD");
                        break;
                    case 6:
                        Console.Write("INC ");
                        contador++;
                        while(segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 7:
                        Console.Write("DEC ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 8:
                        Console.WriteLine("CMPEQ");
                        break;
                    case 9:
                        Console.WriteLine("CMPNE");
                        break;
                    case 10:
                        Console.WriteLine("CMPLT");
                        break;
                    case 11:
                        Console.WriteLine("CMPLE");
                        break;
                    case 12:
                        Console.WriteLine("CMPGT");
                        break;
                    case 13:
                        Console.WriteLine("CMPGE");
                        break;
                    case 14:
                        Console.WriteLine("JMP");
                        contador += 2;
                        break;
                    case 15:
                        Console.WriteLine("JMPT");
                        contador += 2;
                        break;
                    case 16:
                        Console.WriteLine("JMPF");
                        contador += 2;
                        break;
                    case 17:
                        Console.WriteLine("SETIDX");
                        contador += 2;
                        break;
                    case 18:
                        Console.WriteLine("SETIDXK");
                        byte[] baitsSETIDXK = new byte[4];
                        int copiaContadorSETIDXK;
                        copiaContadorSETIDXK = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsSETIDXK[counter] = segcod[copiaContadorSETIDXK];
                            copiaContadorSETIDXK++;
                        }
                        int yeetSETIDXK = BitConverter.ToInt32(baitsSETIDXK, 0);
                        Console.WriteLine("{0}", yeetSETIDXK);
                        contador += 4;
                        break;
                    case 19:
                        Console.Write("PUSHI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 20:
                        Console.Write("PUSHD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 21:
                        Console.Write("PUSHS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 22:
                        Console.Write("PUSHAI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 23:
                        Console.Write("PUSHAD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 24:
                        Console.Write("PUSHAS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 25:
                        Console.Write("PUSHKI ");
                        byte[] baitsPUSHKI = new byte[4];
                        int copiaContadorPUSHKI;
                        copiaContadorPUSHKI = contador + 1;
                        for(int counter = 0; counter < 4; counter++)
                        {
                            baitsPUSHKI[counter] = segcod[copiaContadorPUSHKI];
                            copiaContadorPUSHKI++;
                        }
                        int yeetPUSHKI = BitConverter.ToInt32(baitsPUSHKI, 0);
                        Console.WriteLine("{0}", yeetPUSHKI);
                        contador += 4;
                        break;
                    case 26:
                        Console.Write("PUSHKD ");
                        byte[] baitsPUSHKD = new byte[4];
                        int copiaContadorPUSHKD;
                        copiaContadorPUSHKD = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsPUSHKD[counter] = segcod[copiaContadorPUSHKD];
                            copiaContadorPUSHKD++;
                        }
                        int yeetPUSHKD = BitConverter.ToInt32(baitsPUSHKD, 0);
                        Console.WriteLine("{0}", yeetPUSHKD);
                        contador += 8;
                        break;
                    case 27:
                        Console.Write("PUSHKS ");
                        byte[] baitsPUSHKS = new byte[4];
                        int copiaContadorPUSHKS;
                        copiaContadorPUSHKS = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsPUSHKS[counter] = segcod[copiaContadorPUSHKS];
                            copiaContadorPUSHKS++;
                        }
                        int yeetPUSHKS = BitConverter.ToInt32(baitsPUSHKS, 0);
                        Console.WriteLine("{0}", yeetPUSHKS);
                        contador ++;
                        break;
                    case 28:
                        Console.Write("POPI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 29:
                        Console.Write("POPD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 30:
                        Console.Write("POPS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 31:
                        Console.Write("POPAI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 32:
                        Console.Write("POPAD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 33:
                        Console.Write("POPAS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 34:
                        Console.WriteLine("POPIDX");
                        break;
                    case 35:
                        Console.Write("READI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 36:
                        Console.Write("READD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 37:
                        Console.Write("READS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 38:
                        Console.Write("READAI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 39:
                        Console.Write("READAD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 40:
                        Console.Write("READAS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 41:
                        Console.WriteLine("PRTM");
                        contador++;
                        break;
                    case 42:
                        Console.Write("PRTI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 43:
                        Console.Write("PRTD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 44:
                        Console.Write("PRTS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 45:
                        Console.Write("PRTAI ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 46:
                        Console.Write("PRTAD ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 47:
                        Console.Write("PRTAS ");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 48:
                        Console.WriteLine("HALT");
                        break;
                    default:
                        Console.WriteLine("Instruccion desconocida");
                        break;
                }
            }
        }
    }
}
