using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EnsambladorPrueba
{
    class Program
    {
        class Token
        {
            public string Tipo { get; set; }
            public string Lexema { get; set; }
            public int Dir { get; set; }
            public override string ToString()
            {
                return "Tipo: " + Tipo + "   Lexema: " + Lexema + "   Dir: "+Dir;
            }
        }
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
            { "PRTLN", new Instruccion{ Tamano=1, Codigo=48 }  },

            { "HALT", new Instruccion{ Tamano=1, Codigo=49 }  },
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

        class Pila<T>
        {
            private T[] vector = new T[100];
            private int tope = 0;

            public void Push(T x)
            {
                vector[tope] = x;
                tope++;
            }

            public T Pop()
            {
                tope--;
                return vector[tope];
            }

            public int Count()
            {
                return vector.Length;
            }

            public int Tope()
            {
                return tope;
            }
        }

        private static List<Variable> tabla_var = new List<Variable>();

        private static List<Token> tabla_tokens = new List<Token>();

        private static List<string> lista_errores = new List<string>();

        private static List<string> lista_errores_syntax = new List<string>();

        private static Dictionary<string, int> variables = new Dictionary<string, int>();

        private static Dictionary<string, int> etiquetas_def = new Dictionary<string, int>();//nombre etiqueta, dir

        private static Dictionary<int, string> etiquetas_refer = new Dictionary<int, string>();//tam_seg_cod
        //en etiquetas_ref la llave es la dir porque se pueden repetir las etiquetas

        public static bool EsNombreDeVariable(string lexema)
        {
            bool encontrado = false;
            foreach(Token t in tabla_tokens)
            {
                if(t.Lexema == lexema && t.Tipo == "ident")
                {
                    encontrado = true;
                }
            }
            return encontrado;
        }

        static void Main(string[] args)
        {
            //Aquí empieza Compilador
            string path0 = @"C:/Users/ludmi/Compiladores/cflat-test-1.txt";//La ruta cambia dependiendo de la computadora
            //ruta 1: @"C:/Users/93764/Desktop/pruebas bin/cflat test 1.txt"
            //ruta 2: @"C:/Users/ludmi/Compiladores/cflat-test-1.txt"
            //ruta 3:

            byte[] readTXT = File.ReadAllBytes(path0);
            //para crear el archivo (txt a CFT)
            try
            {
                File.WriteAllBytes("C:/Users/ludmi/Compiladores/cflat test 1 en CFT.CFT", readTXT);
                Console.WriteLine("\n Archivo .CFT creado con éxito.");
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n No se puede crear el archivo CFT.");
                return;
            }

            //Aquí empieza analizador léxico que va a crear los tokens

            byte[] readCFT = File.ReadAllBytes("C:/Users/ludmi/Compiladores/cflat test 1 en CFT.CFT");
            //ruta 1:"C:/Users/93764/Desktop/pruebas bin/cflat test 1 en CFT.CFT"
            //ruta 2:"C:/Users/ludmi/Compiladores/cflat test 1 en CFT.CFT"
            //ruta 3:

            Encoding ascii = Encoding.ASCII;
            string asciiCFT = ascii.GetString(readCFT);

            var lineasCFT = asciiCFT.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.None);

            //Console.WriteLine("Contenido del archivo CFT:\n" + asciiCFT);

            foreach (string st in lineasCFT)
            {
                Console.WriteLine(st.Trim());
            }
            Console.WriteLine();

            //for (var i = 0; i < lineasCFT.Length; i++)//recorre cada línea
            //{
            //    Console.WriteLine(lineasCFT[i]);
            //    string nuevo_token = "";
            //    foreach(char cft in lineasCFT[i])
            //    {
            //        Console.Write(cft+" ");
            //        if (Char.IsWhiteSpace(cft))
            //        {
            //        }
            //    }
            //    Console.WriteLine();
            //}

            char cft;
            string nuevo_lexema = "";
            int estado = 0;//estado del autómata del analizador léxico
            //int numLinea = 1;
            for(var i=0; i<lineasCFT.Length; i++)//lee todo el código CFT de entrada, linea por linea
            {//i es número de línea

                string filaCFT = lineasCFT[i].Trim() + " ";//el espacio es un auxiliar para identificar tokens más fácil
                estado = 0;//cada vez que inicia de nuevo la línea, el estado es 0

                for(var j=0; j<filaCFT.Length; j++)//lee cada char de cada linea
                {//j es número de char (columna)

                    cft = filaCFT[j];//se revisa cada char del código CFT
                    //Console.WriteLine("estado: " + estado);
                    switch (estado)
                    {
                        case 0://inicial, es dígito o letra?
                            if (Char.IsDigit(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 1;
                            }
                            else if (Char.IsLetter(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 3;
                            }
                            else if (cft == '"')
                            {
                                nuevo_lexema += cft;
                                estado = 5;
                            }
                            else if(cft == ' ')
                            {
                                estado = 0;
                            }
                            else if(cft == ';')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "DELIMITADOR", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '[')
                            {
                                nuevo_lexema += cft;
                                estado = 4;
                            }
                            else if (cft == ']')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "ARRAY", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '(')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "PAR_IZQ", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == ')')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "PAR_DER", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == ':')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "DOSPUNTOS", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '{')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "LLA_IZQ", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '}')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "LLA_DER", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '=')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "IGUAL", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == ',')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "COMA", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '+')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "SUMA", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '-')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "RESTA", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '*')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "MULTIPLICACIÓN", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '/')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "DIVISIÓN", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '%')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "DIVISIÓN", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '!')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "DIFERENTE_A", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '>')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "MAYOR_QUE", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if (cft == '<')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "MENOR_QUE", Lexema = nuevo_lexema, Dir = i });
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            else if(cft == '$')
                            {//los comentarios se ignoran y no se guardan como token
                                estado = 7;
                                nuevo_lexema = "";
                            }
                            else
                            {
                                estado = -1;
                                j--;
                            }
                            break;
                        case 1://dígito, continúa (int) o tiene punto (double)?
                            if (Char.IsDigit(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 1;
                            }
                            else if (cft == '.')
                            {
                                nuevo_lexema += cft;
                                estado = 2;
                            }
                            else
                            {
                                //nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "INT", Lexema = nuevo_lexema, Dir = i });
                                j--;
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            break;
                        case 2://double
                            if (Char.IsDigit(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 2;
                            }
                            else
                            {
                                tabla_tokens.Add(new Token() { Tipo = "DOUBLE", Lexema = nuevo_lexema, Dir = i });
                                j--;//para que se regrese a leer el caracter que no se añadió al siguiente lexema
                                estado = 0;
                                nuevo_lexema = "";
                            }
                            break;
                        case 3://letra, es palabra reservada, nombre (identificador) o variable?
                            if (Char.IsLetterOrDigit(cft) || cft == '_')
                            {
                                nuevo_lexema += cft;
                                estado = 3;
                            }
                            else
                            {
                                bool esNombre = EsNombreDeVariable(nuevo_lexema);
                                if (esNombre)//es una variable
                                {
                                    estado = 0;
                                    if (cft == '[')//es una variable de tipo array
                                    {
                                        nuevo_lexema += cft;
                                        j++;
                                        estado = 6;
                                    }
                                    else//es otra variable
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "variable", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                    }
                                    j--;
                                }
                                else//es una palabra reservada (instrucción o tipo) o un identificador (nueva variable)?
                                {
                                    if (nuevo_lexema == "START")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "START", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else if (nuevo_lexema == "END")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "END", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else if (nuevo_lexema == "READ" || nuevo_lexema == "IF" || nuevo_lexema == "ELSE"
                                        || nuevo_lexema == "FOR" || nuevo_lexema == "WHILE"
                                        || nuevo_lexema == "PRINT" || nuevo_lexema == "PRINTNL")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "INSTRUCTION", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else if (nuevo_lexema+cft == "INT " || nuevo_lexema+cft == "DOUBLE " || nuevo_lexema+cft == "STRING ")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "TYPE", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else if (cft == '[')
                                    {
                                        j--;
                                        estado = 0;
                                    }
                                    else if(nuevo_lexema == "AND")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "Y", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else if (nuevo_lexema == "OR")
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "O", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    else//si no es una palabra reservada entonces se toma como identificador
                                    {
                                        tabla_tokens.Add(new Token() { Tipo = "ident", Lexema = nuevo_lexema, Dir = i });
                                        nuevo_lexema = "";
                                        j--;
                                        estado = 0;
                                    }
                                    
                                }
                                
                                
                            }
                            break;
                        case 4://solo para subíndice de array
                            if (Char.IsLetterOrDigit(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 4;
                            }
                            else
                            {
                                estado = 0;
                                j--;
                            }
                            break;
                        case 5://cuando tiene " es un string
                            if(cft == '"')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "STRING", Lexema = nuevo_lexema, Dir = i });
                                nuevo_lexema = "";
                                estado = 0;
                            }
                            else
                            {
                                nuevo_lexema += cft;
                                estado = 5;
                            }
                            break;
                        case 6://solo para subindice de variable array
                            if (Char.IsLetterOrDigit(cft))
                            {
                                nuevo_lexema += cft;
                                estado = 6;
                            }
                            else if(cft == ']')
                            {
                                nuevo_lexema += cft;
                                tabla_tokens.Add(new Token() { Tipo = "variable", Lexema = nuevo_lexema, Dir = i });
                                nuevo_lexema = "";
                                estado = 0;
                            }
                            else
                            {
                                estado = 0;
                                j--;
                            }
                            break;
                        case 7://comentarios $$
                            if(cft != '$')
                            {
                                estado = 7;
                            }
                            else
                            {
                                estado = 0;
                            }
                            break;
                        case -1://caracter no aceptado, error
                            nuevo_lexema += cft;
                            lista_errores.Add("Error en fila "+i+". Caracter no aceptado: "+nuevo_lexema);
                            nuevo_lexema = "";
                            estado = 0;
                            break;
                    }//switch estado
                }//for j char por char
                
                
            }//for i linea por linea CFT

            foreach(var token in tabla_tokens)//imprime información de tokens
            {
                Console.WriteLine(token.ToString());
            }
            Console.WriteLine();
            int num_linea_cft = 0;
            foreach(var token in tabla_tokens)//imprime tokens de analizador léxico
            {
                if(token.Dir > num_linea_cft)
                {
                    Console.WriteLine();
                    num_linea_cft = token.Dir;
                }
                Console.Write("<" + token.Tipo + ">");
            }
            Console.WriteLine();
            foreach(var error in lista_errores)//imprime errores encontrados en analizador léxico
            {
                Console.WriteLine(error);
            }

            //Aquí empieza analizador semántico que revisará el orden de los tokens, si no es correcto imprime error y el programa no continuará

            int x = 0;
            if(tabla_tokens[x].Tipo == "START")
            {
                Instructions(ref x, tabla_tokens.Count);
            }
            else
            {
                lista_errores_syntax.Add("Error en fila " + tabla_tokens[x].Dir + ". " + tabla_tokens[0].Tipo);
            }

            foreach (var error in lista_errores_syntax)//imprime errores encontrados en analizador semántico
            {
                Console.WriteLine(error);
            }

            //Si todo está correcto los tokens se convierten en lenguaje ensamblador y se crea el archivo STN

            string path = @"C:/Users/ludmi/Compiladores/prueba-texto-4-en-ase.ASE";//La ruta cambia dependiendo de la computadora
            //ruta 1:@"C:/Users/93764/Desktop/pruebas bin/prueba texto 2 en ase.ASE"
            //ruta 2:@"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/prueba texto 2 en ase.ASE"
            //ruta 3:@"C:/Users/ludmi/Compiladores/prueba-texto-4-en-ase.ASE"

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

            bw = new BinaryWriter(new FileStream("C:/Users/ludmi/Compiladores/probando stn 4.STN", FileMode.Create));
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stn.STN"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stn.STN"
            //ruta destino 3:"C:/Users/ludmi/Compiladores/probando stn 4.STN"

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
                var num_Linea = i + 1;
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
                            bw.Write((byte)palabras_linea[j + 1].Length);//escribe cuántas letras tiene la constante string
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
                            string[] copiaLineasPRTM = lineas[i].Split("PRTM ");
                            copiaLineasPRTM[1] = copiaLineasPRTM[1].Replace("\"","");
                            //Console.WriteLine("STRING SEPARADO: " + copiaLineasPRTM[1].Length);
                            //foreach(char aux in copiaLineasPRTM[1])
                            //{
                            //    Console.Write("-"+aux);
                            //}
                            //Console.WriteLine();
                            bw.Write((byte)copiaLineasPRTM[1].Length);//escribe cuántas letras tiene el mensaje
                            tam_seg_cod += 2;
                            dir += 2;
                            foreach (char c in copiaLineasPRTM[1]) //+n bytes
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
                        case "PRTLN":
                            bw.Write((byte)instrucciones["PRTLN"].Codigo);
                            tam_seg_cod++;
                            dir++;
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

            string path2 = "C:/Users/ludmi/Compiladores/probando stn 4.STN";
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stn.STN"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stn.STN"
            //ruta destino 3:"C:/Users/ludmi/Compiladores/probando stn 4.STN"
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

            Dictionary<String, Object> variables_generadas = new Dictionary<string, object>();//guarda variables para máquina virtual
            var obj = new Object();//este objeto servirá para crear cada variable
            foreach (Variable v in tabla_var)
            {
                string objnombre = v.Nombre;
                string objtipo = v.Tipo;
                string objnumelementos = v.numeroElementos;

                switch (objtipo)
                {
                    case "int":
                        obj = new int();
                        break;
                    case "double":
                        obj = new double();
                        break;
                    case "string":
                        obj = new string("");
                        break;
                    case "array int":
                        obj = new int[Convert.ToInt32(objnumelementos)];
                        break;
                    case "array double":
                        obj = new double[Convert.ToInt32(objnumelementos)];
                        break;
                    case "array string":
                        obj = new string[Convert.ToInt32(objnumelementos)];
                        break;
                    default:
                        obj = null;
                        break;
                }
                if (obj != null)
                    variables_generadas.Add(objnombre, obj);
            }
            //variables_generadas["i"] = -1 + (int)variables_generadas["i"];//prueba de inicializacion de variables
            //variables_generadas["MATRICULA"] = new int[] {123, 456};//prueba de inicializacion de variables ESTE NO SIRVE

            foreach (var v in variables_generadas)//Imprime variables generadas para probar
            {
                Console.WriteLine(v.ToString());
                if(v.Value.ToString().Equals("System.Int32[]"))//Si es array int
                {
                    int[] copiaArrayInt = (int[])v.Value;
                    for (int j=0; j< copiaArrayInt.Length; j++)
                    {
                        Console.WriteLine(copiaArrayInt[j]);
                    }
                }
                else if (v.Value.ToString().Equals("System.Double[]"))//Si es array double
                {
                    double[] copiaArrayDouble = (double[])v.Value;
                    for (int j = 0; j < copiaArrayDouble.Length; j++)
                    {
                        Console.WriteLine(copiaArrayDouble[j]);
                    }
                }
                else if (v.Value.ToString().Equals("System.String[]"))//Si es array string
                {
                    string[] copiaArrayString = (string[])v.Value;
                    for (int j = 0; j < copiaArrayString.Length; j++)
                    {
                        Console.WriteLine(copiaArrayString[j]);
                    }
                }
            }
            

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
            string path3 = "C:/Users/ludmi/Compiladores/probando stnv 4.STNV";
            //ruta destino 1:"C:/Users/93764/Desktop/pruebas bin/probando stnv.STNV"
            //ruta destino 2:"D:/OneDrive - Instituto Educativo del Noroeste, A.C/Docs/CETYS/Universidad/7mo/Compiladores/Programas/ensamblador/probando stnv.STNV"
            //ruta destino 3:"C:/Users/ludmi/Compiladores/probando stnv 4.STNV"
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

            foreach(var etiquetad in etiquetas_def)
            {
                Console.WriteLine("EtiquetaDef {0}, {1}", etiquetad.Key, etiquetad.Value);
            }
            foreach (var etiquetaref in etiquetas_refer)
            {
                Console.WriteLine("EtiquetaRefer {0}, {1}", etiquetaref.Key, etiquetaref.Value);
            }


            int conta;
            for (contador = 12; contador < tam_seg_cod; contador++)//recorre el array, compara con el codigo de cada instruccion
            {                                                     //y escribe la instruccion que corresponde
                conta = 0;
                switch (segcod[contador])//Imprime la instruccion segun el codigo de operacion, y aumenta el espacio recorrido segun la instruccion
                {
                    case 0:
                        Console.WriteLine("NOP");
                        break;
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
                        Console.Write("JMP ");
                        contador++;
                        Console.Write(segcod[contador] + "\n");
                        contador++;
                        break;
                    case 15:
                        Console.Write("JMPT ");
                        contador++;
                        Console.Write(segcod[contador] + "\n");
                        contador++;
                        break;
                    case 16:
                        Console.Write("JMPF ");
                        contador++;
                        Console.Write(segcod[contador] + "\n");
                        contador++;
                        break;
                    case 17:
                        Console.WriteLine("SETIDX");
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.WriteLine(varias[conta].Nombre);
                        contador++;
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
                        int cantBytes = segcod[contador + 1];
                        contador += 2;
                        for(int contPUSHKS = 0; contPUSHKS < cantBytes; contPUSHKS++)
                        {
                            char c2 = Convert.ToChar(segcod[contador]);
                            Console.Write(c2);
                            contador++;
                        }
                        contador--;
                        Console.WriteLine("");
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
                        Console.Write("PRTM ");
                        int cantdeBytes = segcod[contador + 1];
                        contador += 2;
                        for (int contPUSHKS = 0; contPUSHKS < cantdeBytes; contPUSHKS++)
                        {
                            char c2 = Convert.ToChar(segcod[contador]);
                            Console.Write(c2);
                            contador++;
                        }
                        contador--;
                        Console.WriteLine("");
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
                        Console.WriteLine("PRTLN");
                        break;
                    case 49:
                        Console.WriteLine("HALT");
                        break;
                    default:
                        Console.WriteLine("Instruccion desconocida");
                        break;
                }//switch
            }//for

            //aquí empieza máquina virtual
            Console.WriteLine("-------------- MÁQUINA VIRTUAL--------------\n");
            Pila<Object> pila_main = new Pila<Object>();//pila principal

            //var obj_aux = new Object();//objeto auxiliar para la variable en la instrucción NO SE OCUPA AHORITA
            int idx_array = 0;
            bool boolean_main = false;
            int[] copiaArrayIntMV;
            double[] copiaArrayDoubleMV;
            string[] copiaArrayStringMV;

            for (contador = 12; contador < tam_seg_cod; contador++)//recorre el array, compara con el codigo de cada instruccion
            {                                                     //y simula lo que debe realizar la instrucción
                conta = 0;
                switch (segcod[contador])//Imprime la instruccion segun el codigo de operacion, y aumenta el espacio recorrido segun la instruccion
                {
                    case 0://NOP
                        //Console.WriteLine("--NOP--");
                        break;
                    case 1://ADD
                        var add2 = pila_main.Pop();
                        var add1 = pila_main.Pop();
                        var add_res = new Object();
                        if(add1.GetType().Equals(typeof(string)) || add2.GetType().Equals(typeof(string)))
                        {//si alguno de los dos es string
                            add_res = (string)add2 + (string)add1;
                        }
                        else if (add1.GetType().Equals(typeof(int)))//si el primero es int
                        {
                            add_res = (int)add2 + (int)add1;
                        }
                        else if (add1.GetType().Equals(typeof(double)))//si el primero es double
                        {
                            add_res = (double)add2 + (double)add1;
                        }
                        pila_main.Push(add_res);
                        break;
                    case 2://SUB
                        var sub2 = pila_main.Pop();
                        var sub1 = pila_main.Pop();
                        var sub_res = new Object();
                        if (sub1.GetType().Equals(typeof(int)))//si el primero es int
                        {
                            sub_res = (int)sub2 - (int)sub1;
                        }
                        else if (sub1.GetType().Equals(typeof(double)))//si el primero es double
                        {
                            sub_res = (double)sub2 - (double)sub1;
                        }
                        pila_main.Push(sub_res);
                        break;
                    case 3://MULT
                        var mult2 = pila_main.Pop();
                        var mult1 = pila_main.Pop();
                        var mult_res = new Object();
                        if (mult1.GetType().Equals(typeof(int)))//si el primero es int
                        {
                            mult_res = (int)mult2 * (int)mult1;
                        }
                        else if (mult1.GetType().Equals(typeof(double)))//si el primero es double
                        {
                            mult_res = (double)mult2 * (double)mult1;
                        }
                        pila_main.Push(mult_res);
                        break;
                    case 4://DIV
                        var div2 = pila_main.Pop();
                        var div1 = pila_main.Pop();
                        var div_res = new Object();
                        if (div1.GetType().Equals(typeof(int)))//si el primero es int
                        {
                            div_res = (int)div2 / (int)div1;
                        }
                        else if (div1.GetType().Equals(typeof(double)))//si el primero es double
                        {
                            div_res = (double)div2 / (double)div1;
                        }
                        pila_main.Push(div_res);
                        break;
                    case 5://MOD
                        var mod2 = pila_main.Pop();
                        var mod1 = pila_main.Pop();
                        var mod_res = new Object();
                        if (mod1.GetType().Equals(typeof(int)))//si el primero es int
                        {
                            mod_res = (int)mod2 % (int)mod1;
                        }
                        else if (mod1.GetType().Equals(typeof(double)))//si el primero es double
                        {
                            mod_res = (double)mod2 % (double)mod1;
                        }
                        pila_main.Push(mod_res);
                        break;
                    case 6://INC
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = 1 + (int)variables_generadas[varias[conta].Nombre];
                        //Console.WriteLine(variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        
                        break;
                    case 7://DEC
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = (int)variables_generadas[varias[conta].Nombre] - 1;
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 8://CMPEQ
                        var cmpeq2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmpeq2
                        var cmpeq1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmpeq1
                        if (cmpeq1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmpeq1 == (double)cmpeq2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmpeq1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmpeq1 == (int)cmpeq2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        break;
                    case 9://CMPNE
                        var cmpne2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmpne2
                        var cmpne1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmpne1
                        if (cmpne1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmpne1 != (double)cmpne2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmpne1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmpne1 != (int)cmpne2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        break;
                    case 10://CMPLT
                        var cmplt2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmplt2
                        var cmplt1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmplt1
                        //Console.WriteLine("cmplt1: {0}, cmplt2: {1}", cmplt1, cmplt2);
                        if (cmplt1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmplt1 < (double)cmplt2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmplt1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmplt1 < (int)cmplt2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                            //Console.WriteLine("bool en JMPT: " + boolean_main);
                        }
                        break;
                    case 11://CMPLE
                        var cmple2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmple2
                        var cmple1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmple1
                        if (cmple1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmple1 <= (double)cmple2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmple1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmple1 <= (int)cmple2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        break;
                    case 12://CMPGT
                        var cmpgt2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmpgt2
                        var cmpgt1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmpgt1
                        if (cmpgt1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmpgt1 > (double)cmpgt2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmpgt1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmpgt1 > (int)cmpgt2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        break;
                    case 13://CMPGE
                        var cmpge2 = pila_main.Pop();//se guarda el valor del tope de la pila en cmpge2
                        var cmpge1 = pila_main.Pop();//se guarda el siguiente valor del tope de la pila en cmpge1
                        if (cmpge1.GetType().Equals(typeof(double)))
                        {
                            if ((double)cmpge1 >= (double)cmpge2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        else if (cmpge1.GetType().Equals(typeof(int)))
                        {
                            if ((int)cmpge1 >= (int)cmpge2)
                            {
                                boolean_main = true;
                            }
                            else
                            {
                                boolean_main = false;
                            }
                        }
                        break;
                    case 14://JMP
                        contador++;//se mueve al byte de la linea a la que se salta con JMP
                        contador = (segcod[contador] + 11);//11 y no 12 bytes porque el for ya hace un contador++
                        //Console.Write(segcod[contador] + "\n");
                        //contador++;
                        break;
                    case 15://JMPT
                        contador++;//se mueve al byte de la linea a la que se salta con JMPT
                        //Console.WriteLine("bool: "+boolean_main);
                        //Console.WriteLine("idx_array: " + idx_array);
                        //Console.WriteLine("segcod en JMPT: " + (segcod[contador]));
                        if (boolean_main)
                        {
                            contador = (segcod[contador]+11);//11 y no 12 bytes porque el for ya hace un contador++
                        }
                        else
                        {
                            contador++;//se salta el byte que queda de la linea que se tiene que saltar (son 2 bytes en total)
                        }
                        //Console.WriteLine("contador: " + contador);
                        //Console.WriteLine("segcod en JMPT (2): " + (segcod[contador]));
                        //Console.Write(segcod[contador] + "\n");
                        break;
                    case 16://JMPF
                        contador++;//se mueve al byte de la linea a la que se salta con JMPF
                        //Console.Write(segcod[contador] + "\n");
                        if (boolean_main == false)
                        {
                            contador = (segcod[contador] + 11);//11 y no 12 bytes porque el for ya hace un contador++
                        }
                        else
                        {
                            contador++;//se salta el byte que queda de la linea que se tiene que saltar (son 2 bytes en total)
                        }
                        break;
                    case 17://SETIDX
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        idx_array = (int)variables_generadas[varias[conta].Nombre];
                        contador++;
                        break;
                    case 18://SETIDXK
                        byte[] baitsSETIDXK = new byte[4];
                        int copiaContadorSETIDXK;
                        copiaContadorSETIDXK = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsSETIDXK[counter] = segcod[copiaContadorSETIDXK];
                            copiaContadorSETIDXK++;
                        }
                        int yeetSETIDXK = BitConverter.ToInt32(baitsSETIDXK, 0);
                        //Console.WriteLine("{0}", yeetSETIDXK);
                        idx_array = yeetSETIDXK;
                        contador += 4;
                        break;
                    case 19://PUSHI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        //Console.WriteLine("i:" + variables_generadas[varias[conta].Nombre]);
                        pila_main.Push(variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine("------PUSHI");
                        //Console.WriteLine(varias[conta].Nombre);
                        //Console.WriteLine("segcod[contador] en PUSHI: " + segcod[contador]);
                        contador++;
                        //Console.WriteLine("segcod[contador] en PUSHI(2): " + segcod[contador]);
                        //Console.WriteLine(varias[conta].Nombre);
                        break;
                    case 20://PUSHD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        pila_main.Push(variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 21://PUSHS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        pila_main.Push(variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 22://PUSHAI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        pila_main.Push((int[])variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 23://PUSHAD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        pila_main.Push((double[])variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 24://PUSHAS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        pila_main.Push((string[])variables_generadas[varias[conta].Nombre]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 25://PUSHKI
                        byte[] baitsPUSHKI = new byte[4];
                        int copiaContadorPUSHKI;
                        copiaContadorPUSHKI = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsPUSHKI[counter] = segcod[copiaContadorPUSHKI];
                            copiaContadorPUSHKI++;
                        }
                        int yeetPUSHKI = BitConverter.ToInt32(baitsPUSHKI, 0);
                        //Console.WriteLine("{0}", yeetPUSHKI);
                        pila_main.Push(yeetPUSHKI);//se inserta en la pila el valor int
                        contador += 4;
                        break;
                    case 26://PUSHKD
                        byte[] baitsPUSHKD = new byte[4];
                        int copiaContadorPUSHKD;
                        copiaContadorPUSHKD = contador + 1;
                        for (int counter = 0; counter < 4; counter++)
                        {
                            baitsPUSHKD[counter] = segcod[copiaContadorPUSHKD];
                            copiaContadorPUSHKD++;
                        }
                        double yeetPUSHKD = BitConverter.ToDouble(baitsPUSHKD, 0);
                        //Console.WriteLine("{0}", yeetPUSHKD);
                        pila_main.Push(yeetPUSHKD);//se inserta en la pila el valor double
                        contador += 8;
                        break;
                    case 27://PUSHKS
                        int cantBytes = segcod[contador + 1];
                        contador += 2;
                        string yeetPUSHKS = "";
                        for (int contPUSHKS = 0; contPUSHKS < cantBytes; contPUSHKS++)
                        {
                            char c2 = Convert.ToChar(segcod[contador]);
                            //Console.Write(c2);
                            yeetPUSHKS += c2;
                            contador++;
                        }
                        pila_main.Push(yeetPUSHKS);//se inserta en la pila el valor string
                        contador--;
                        //Console.WriteLine("");
                        break;
                    case 28://POPI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = int.Parse(pila_main.Pop().ToString());//se hace pop y se guarda en la variable
                        //Console.WriteLine("------------------POPI");
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 29://POPD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = double.Parse(pila_main.Pop().ToString());//se hace pop y se guarda en la variable
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 30://POPS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = pila_main.Pop().ToString();//se hace pop y se guarda en la variable
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 31://POPAI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = (int[])pila_main.Pop();//se hace pop y se guarda en la variable
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 32://POPAD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = (double[])pila_main.Pop();//se hace pop y se guarda en la variable
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 33://POPAS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = (string[])pila_main.Pop();//se hace pop y se guarda en la variable
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 34://POPIDX
                        idx_array = int.Parse(pila_main.Pop().ToString());//se hace pop y se guarda en idx_array
                        //Console.WriteLine("POPIDX: "+idx_array);
                        break;
                    case 35://READI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = Convert.ToInt32(Console.ReadLine());
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 36://READD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = Convert.ToDouble(Console.ReadLine());
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 37://READS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        variables_generadas[varias[conta].Nombre] = Convert.ToString(Console.ReadLine());
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 38://READAI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        //int intTemp = Convert.ToInt32(Console.ReadLine());//lee el numero que se ingresa en el teclado
                        copiaArrayIntMV = (int[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        //Console.Write("Inserte un valor para el arreglo {0} en [{1}]: ", varias[conta].Nombre, idx_array);
                        copiaArrayIntMV[idx_array] = Convert.ToInt32(Console.ReadLine());//en el idx correspondiente del arreglo guarda el valor int ingresado
                        variables_generadas[varias[conta].Nombre] = copiaArrayIntMV;//la copia modificada se vuelve a poner en el diccionario
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 39://READAD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        copiaArrayDoubleMV = (double[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        //Console.Write("Inserte un valor para el arreglo {0} en [{1}]: ", varias[conta].Nombre, idx_array);
                        copiaArrayDoubleMV[idx_array] = Convert.ToDouble(Console.ReadLine());//en el idx correspondiente del arreglo guarda el valor int ingresado
                        variables_generadas[varias[conta].Nombre] = copiaArrayDoubleMV;//la copia modificada se vuelve a poner en el diccionario
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 40://READAS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        copiaArrayStringMV = (string[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        //Console.Write("Inserte un valor para el arreglo {0} en [{1}]: ", varias[conta].Nombre, idx_array);
                        copiaArrayStringMV[idx_array] = Convert.ToString(Console.ReadLine());//en el idx correspondiente del arreglo guarda el valor int ingresado
                        variables_generadas[varias[conta].Nombre] = copiaArrayStringMV;//la copia modificada se vuelve a poner en el diccionario
                        contador++;

                        //Console.WriteLine("segcod[contador] en READAS: " + segcod[contador]);
                        //Console.WriteLine(varias[conta].Nombre);
                        break;
                    case 41://PRTM
                        int cantdeBytes = segcod[contador + 1];
                        contador += 2;
                        for (int contPUSHKS = 0; contPUSHKS < cantdeBytes; contPUSHKS++)
                        {
                            char c2 = Convert.ToChar(segcod[contador]);
                            Console.Write(c2);
                            contador++;
                        }
                        contador--;
                        //Console.WriteLine("");
                        break;
                    case 42://PRTI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.Write(varias[conta]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 43://PRTD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.Write(varias[conta]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 44://PRTS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        Console.Write(varias[conta]);
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 45://PRTAI
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        copiaArrayIntMV = (int[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        //Console.WriteLine("{0}[{1}]: {2} \n", varias[conta].Nombre, idx_array, copiaArrayIntMV[idx_array]);//imprime el elemento del arreglo en idx_array
                        Console.Write(copiaArrayIntMV[idx_array]);//imprime el elemento del arreglo en idx_array
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 46://PRTAD
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        copiaArrayDoubleMV = (double[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        Console.Write(copiaArrayDoubleMV[idx_array]);//imprime el elemento del arreglo en idx_array
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 47://PRTAS
                        contador++;
                        while (segcod[contador] != varias[conta].Direccion)
                        {
                            conta++;
                        }
                        copiaArrayStringMV = (string[])variables_generadas[varias[conta].Nombre];//se guarda en una copia el arreglo del diccionario
                        Console.Write(copiaArrayStringMV[idx_array]);//imprime el elemento del arreglo en idx_array
                        //Console.WriteLine(varias[conta].Nombre);
                        contador++;
                        break;
                    case 48://PRTLN
                        Console.WriteLine();
                        break;
                    case 49://HALT
                        contador = tam_seg_cod;
                        break;
                    default:
                        Console.WriteLine("ERROR");
                        break;
                }//switch
            }//for

        }//static void main

        public static bool Instructions(ref int pos, int length)
        {
            
            pos++;
            int posTemp = pos;

            if (pos == length - 1)
            {
                return true;
            }

            switch (tabla_tokens[pos].Tipo)
            {
                //Declaración de variables
                case "ARRAY":
                case "TYPE":
                    pos++;
                    if(tabla_tokens[pos].Tipo != "ident")
                    {
                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                    }

                    posTemp = pos;
                    if(!instructionsCiclos("CI", ref pos, length))
                    { 
                        pos = posTemp;
                        pos++;
                        if (tabla_tokens[pos].Tipo != "DELIMITADOR")
                        {
                            lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                        }
                    }

                    
                        break;
                    //asignar valor y operaciones
                case "variable":
                    pos++;
                    if (tabla_tokens[pos].Tipo != "IGUAL")
                    {
                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                    }

                    posTemp = pos;
                    if(!instructionsCiclos("OPERANDOS", ref pos, length)) 
                    {
                        pos = posTemp;
                        if (!instructionsCiclos("OPERACION", ref pos, length))
                        {
                            lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                        }
                    }

                    pos++;
                    if (tabla_tokens[pos].Tipo != "DELIMITADOR")
                    {
                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                    }
                    break;
                case "INSTRUCTION":
                    posTemp = pos;
                    pos++;
                    if (tabla_tokens[pos].Tipo == "PAR_IZQ")
                    {
                        posTemp = pos;
                        //IF/WHILE
                        if (instructionsCiclos("COND", ref pos, length))
                        {
                            instructionsCiclos("PARLLA", ref pos, length);
                        }
                        //FOR
                        else 
                        {
                            pos = posTemp;
                            pos++;
                            if (tabla_tokens[pos].Tipo == "variable")
                            {
                                pos++;
                                if (tabla_tokens[pos].Tipo == "DOSPUNTOS")
                                {
                                    pos++;
                                    if (tabla_tokens[pos].Tipo == "variable" || tabla_tokens[pos].Tipo == "INT")
                                    {
                                        instructionsCiclos("PARLLA", ref pos, length);
                                    }
                                    else
                                    {
                                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                                    }
                                }
                                else
                                {
                                    lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                                }
                            }
                            else
                            {
                                lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                            }
                        }
                        
                    }
                    else 
                    {
                        //PRINT/PRINTNL/READ
                        pos = posTemp;
                        pos++;
                        if (tabla_tokens[pos].Tipo == "variable" || tabla_tokens[pos].Tipo == "STRING")
                        {
                            instructionsCiclos("CVS", ref pos, length);

                            pos++;
                            if (tabla_tokens[pos].Tipo != "DELIMITADOR")
                            {
                                lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                            }
                        }
                        else
                        {
                            pos = posTemp;
                            pos++;
                            if (tabla_tokens[pos].Tipo != "DELIMITADOR")
                            {
                                lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                            }
                            
                        }
                    }
                    break;
                case "LLA_DER":
                    return true;
                    break;
                case "END":
                    return true;
                    break;
            }

                return Instructions(ref pos, length); ;
        }

        public static bool instructionsCiclos(string gram, ref int pos, int length)
        {
            int postTemp = 0;
            switch (gram)
            {
                case "CI":
                    pos++;
                    if(tabla_tokens[pos].Tipo == "COMA")
                    {
                        pos++;
                        if (tabla_tokens[pos].Tipo == "ident")
                        {
                            instructionsCiclos("CI", ref pos, length);
                        }
                        else
                        {
                            lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                        }
                    }
                    break;
                case "OPERACION":
                    postTemp = pos;
                    if(instructionsCiclos("OPERANDO", ref pos, length))
                    {
                        instructionsCiclos("OP", ref pos, length);
                    }
                    else 
                    {
                        pos = postTemp;
                        pos++;
                        if (tabla_tokens[pos].Tipo == "PAR_IZQ")
                        {
                            if (instructionsCiclos("OPERACION", ref pos, length))
                            {
                                pos++;
                                if (tabla_tokens[pos].Tipo == "PAR_DER")
                                {
                                    instructionsCiclos("OP", ref pos, length);
                                }
                            }
                        }
                    }
                    break;
                case "OP":
                    postTemp = pos;
                    if (instructionsCiclos("OPERADOR", ref pos, length))
                    {
                        instructionsCiclos("OPERACION", ref pos, length);
                    }
                    else
                    {
                        pos = postTemp;
                    }
                    break;
                case "OPERANDO":
                    pos++;
                    if(tabla_tokens[pos].Tipo == "variable" || tabla_tokens[pos].Tipo == "INT" || tabla_tokens[pos].Tipo == "DOUBLE")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                        break;
                    //Extension de OPERANDO, se permite unir cadenas de string
                case "OPERANDOS":
                    if (instructionsCiclos("OPERANDO", ref pos, length) || tabla_tokens[pos].Tipo == "STRING")
                    {
                        pos++;
                        if (tabla_tokens[pos].Tipo == "SUMA")
                        {
                            postTemp = pos;
                            if(!instructionsCiclos("OPERANDOS", ref pos, length))
                            {
                                pos = postTemp;
                                if (!instructionsCiclos("OPERACION", ref pos, length))
                                {
                                    lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                case "OPERADOR":
                    pos++;
                    if (tabla_tokens[pos].Tipo == "SUMA" || tabla_tokens[pos].Tipo == "RESTA" || tabla_tokens[pos].Tipo == "MULTIPLICACION" || tabla_tokens[pos].Tipo == "DIVISION")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                    //COMA, VARIABLE/STRING (PRINT y READ)
                case "CVS":
                    postTemp = pos;
                    pos++;
                    if (tabla_tokens[pos].Tipo == "COMA")
                    {
                        pos++;
                        if (tabla_tokens[pos].Tipo == "STRING" || tabla_tokens[pos].Tipo == "variable")
                        {
                            instructionsCiclos("CVS", ref pos, length);
                        }
                        else
                        {
                            lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                        }
                    }
                    else
                    {
                        pos = postTemp;
                    }
                    break;
                    //CONDICION (<, >, =, !)
                case "COND":
                    if(instructionsCiclos("OPERANDO", ref pos, length))
                    {
                        pos++;
                        if (tabla_tokens[pos].Tipo == "IGUAL" || tabla_tokens[pos].Tipo == "DIFERENTE_A" || tabla_tokens[pos].Tipo == "MAYOR_QUE" || tabla_tokens[pos].Tipo == "MENOR_QUE")
                        {
                            instructionsCiclos("OPERANDO", ref pos, length);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                    }
                    break;
                    //AND y OR
                case "AO":
                    pos++;
                    if (tabla_tokens[pos].Tipo == "A" || tabla_tokens[pos].Tipo == "O")
                    {
                        instructionsCiclos("COND", ref pos, length);
                    }
                        break;
                    //Parentesis y llaves (FOR y WHILE)
                case "PARLLA":
                    pos++;
                    if (tabla_tokens[pos].Tipo == "PAR_DER")
                    {
                        pos++;
                        if (tabla_tokens[pos].Tipo == "LLA_IZQ")
                        {
                            Instructions(ref pos, length);

                            if (tabla_tokens[pos].Tipo != "LLA_DER")
                            {
                                lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                            }
                        }
                        else
                        {
                            lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                        }
                    }
                    else
                    {
                        lista_errores_syntax.Add("Error en fila " + tabla_tokens[pos].Dir + ". " + tabla_tokens[pos].Tipo);
                    }
                    break;
            }

            return true;
        }
    }//Program
}//EnsambladorPrueba
