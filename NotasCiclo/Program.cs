using System;
using System.Text.RegularExpressions;

namespace SistemaGestionAcademica
{
    class Program
    {
        // Límite máximo de estudiantes
        const int MAX_ESTUDIANTES = 50; //[cite: 1]
        static int totalEstudiantes = 0;

        // Arreglos principales para guardar la información de los estudiantes
        static string[] nombres = new string[MAX_ESTUDIANTES];
        static string[] generos = new string[MAX_ESTUDIANTES];
        static string[] carnets = new string[MAX_ESTUDIANTES];
        static string[] facultades = new string[MAX_ESTUDIANTES];
        static string[] carreras = new string[MAX_ESTUDIANTES];

        // Datos de materias y notas
        static string[] nombresMaterias = new string[MAX_ESTUDIANTES];
        static double[,] notasPeriodos = new double[MAX_ESTUDIANTES, 3];
        static double[,] asistencias = new double[MAX_ESTUDIANTES, 3];
        static double[] promediosFinales = new double[MAX_ESTUDIANTES];
        static double[] asistenciasPromedio = new double[MAX_ESTUDIANTES];
        static bool[] aprobados = new bool[MAX_ESTUDIANTES];

        static void Main(string[] args)
        {
            int opcion = 0;

            do
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("   SISTEMA DE GESTIÓN ACADÉMICA INTELIGENTE UNICAES");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. Registrar Estudiante");
                Console.WriteLine("2. Mostrar Estudiantes");
                Console.WriteLine("3. Buscar Estudiante (por Carnet)");
                Console.WriteLine("4. Modificar Notas y Asistencias");
                Console.WriteLine("5. Eliminar Estudiante");
                Console.WriteLine("6. Estadísticas Generales");
                Console.WriteLine("7. Top 3 Estudiantes");
                Console.WriteLine("8. Reportes");
                Console.WriteLine("9. Salir");
                Console.WriteLine("==================================================");

                opcion = LeerEnteroValido("Seleccione una opción (1-9): ", 1, 9);

                switch (opcion)
                {
                    case 1: RegistrarEstudiante(); break;
                    case 2: MostrarEstudiantes(); break;
                    case 3: BuscarEstudiante(); break;
                    case 4: ModificarNotasAsistencias(); break;
                    case 5: EliminarEstudiante(); break;
                    case 6: EstadisticasGenerales(); break;
                    case 7: Top3Estudiantes(); break;
                    case 8: Reportes(); break;
                    case 9: Console.WriteLine("\n¡Gracias por usar el sistema!"); break;
                }

                if (opcion != 9)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }

            } while (opcion != 9);
        }

        // ==========================================
        // 1. REGISTRAR ESTUDIANTE
        // ==========================================
        static void RegistrarEstudiante()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRAR NUEVO ESTUDIANTE ---");

            if (totalEstudiantes >= MAX_ESTUDIANTES)
            {
                Console.WriteLine("Error: Se ha alcanzado el límite máximo de 50 estudiantes."); //[cite: 1]
                return;
            }

            int i = totalEstudiantes;

            // Datos del estudiante con validaciones estrictas
            nombres[i] = LeerSoloTexto("Ingrese el nombre completo (solo letras): ");
            generos[i] = LeerGeneroValido();
            carnets[i] = LeerCarnetValido();

            // Selección de Facultad y Carrera mediante menú numérico
            SeleccionarFacultadyCarrera(i);

            // Nombre de la Materia
            nombresMaterias[i] = LeerTextoAlfanumerico("Ingrese el nombre de la materia a registrar: ");

            double sumaNotasMaterias = 0;
            double sumaAsistencias = 0;

            for (int p = 0; p < 3; p++) //[cite: 1]
            {
                Console.WriteLine($"\n--- PERÍODO {p + 1} ---");

                // Nota Examen Parcial (50%)
                double parcial = LeerDoubleValido("Ingrese nota del Examen Parcial (0-10): ", 0, 10);

                // Actividades Evaluadas (50%)
                Console.WriteLine("Configuración de Actividades (deben sumar 50% en total):");
                int numActividades = LeerEnteroValido("¿Cuántas actividades evaluadas realiza en este período?: ", 1, 10);

                double notaActividadesAcumulada = 0;
                double porcentajeAcumulado = 0;

                for (int act = 0; act < numActividades; act++)
                {
                    Console.WriteLine($"\nActividad #{act + 1}:");
                    double notaAct = LeerDoubleValido("  Nota de la actividad (0-10): ", 0, 10);

                    double porcAct = 0;
                    if (act == numActividades - 1)
                    {
                        porcAct = 50.0 - porcentajeAcumulado;
                        Console.WriteLine($"  Porcentaje asignado automáticamente para sumar 50%: {porcAct}%");
                    }
                    else
                    {
                        double maxPermitido = 50.0 - porcentajeAcumulado - (numActividades - 1 - act);
                        porcAct = LeerDoubleValido($"  Porcentaje de esta actividad (1-{maxPermitido}%): ", 1, maxPermitido);
                        porcentajeAcumulado += porcAct;
                    }

                    notaActividadesAcumulada += notaAct * (porcAct / 100.0);
                }

                double notaPeriodo = (parcial * 0.50) + notaActividadesAcumulada;
                notasPeriodos[i, p] = notaPeriodo;
                sumaNotasMaterias += notaPeriodo;

                double asis = LeerDoubleValido($"Ingrese porcentaje de asistencia del Período {p + 1} (0-100%): ", 0, 100);
                asistencias[i, p] = asis;
                sumaAsistencias += asis;
            }

            promediosFinales[i] = sumaNotasMaterias / 3.0;
            asistenciasPromedio[i] = sumaAsistencias / 3.0;

            // Criterio de Aprobación: Nota >= 6.0 y Asistencia >= 75%
            aprobados[i] = (promediosFinales[i] >= 6.0) && (asistenciasPromedio[i] >= 75.0); //[cite: 1]

            totalEstudiantes++;
            Console.WriteLine("\n¡Estudiante registrado con éxito!");
        }

        // ==========================================
        // 2. MOSTRAR ESTUDIANTES
        // ==========================================
        static void MostrarEstudiantes()
        {
            Console.Clear();
            Console.WriteLine("--- MOSTRAR ESTUDIANTES ---");

            if (totalEstudiantes == 0)
            {
                Console.WriteLine("No hay estudiantes registrados.");
                return;
            }

            Console.WriteLine("1. Mostrar Todos");
            Console.WriteLine("2. Filtrar por Facultad");
            Console.WriteLine("3. Filtrar por Carrera");
            int subOp = LeerEnteroValido("Seleccione opción (1-3): ", 1, 3);

            int facFiltro = 0, carFiltro = 0;
            string nombreFacultadFiltro = "", nombreCarreraFiltro = "";

            if (subOp == 2)
            {
                facFiltro = SeleccionarSoloFacultad();
                nombreFacultadFiltro = ObtenerSiglaFacultad(facFiltro);
            }
            else if (subOp == 3)
            {
                facFiltro = SeleccionarSoloFacultad();
                carFiltro = SeleccionarSoloCarrera(facFiltro);
                nombreCarreraFiltro = ObtenerNombreCarrera(facFiltro, carFiltro);
            }

            Console.WriteLine("\n--------------------------------------------------------------------------------------------------");
            Console.WriteLine("Carnet\t\tNombre\t\tFacultad\tCarrera\t\tNota Final\tAsistencia\tEstado");
            Console.WriteLine("--------------------------------------------------------------------------------------------------");

            for (int i = 0; i < totalEstudiantes; i++)
            {
                bool mostrar = false;
                if (subOp == 1) mostrar = true;
                else if (subOp == 2 && facultades[i] == nombreFacultadFiltro) mostrar = true;
                else if (subOp == 3 && carreras[i] == nombreCarreraFiltro) mostrar = true;

                if (mostrar)
                {
                    string estado = aprobados[i] ? "APROBADO" : "REPROBADO";
                    Console.WriteLine($"{carnets[i]}\t{nombres[i]}\t{facultades[i]}\t{carreras[i]}\t{promediosFinales[i]:F2}\t\t{asistenciasPromedio[i]:F1}%\t\t{estado}");
                }
            }
        }

        // ==========================================
        // 3. BUSCAR ESTUDIANTE (POR CARNET)
        // ==========================================
        static void BuscarEstudiante()
        {
            Console.Clear();
            Console.WriteLine("--- BUSCAR ESTUDIANTE ---");
            string carnetBuscado = LeerCarnetValido();

            int pos = BuscarPorCarnet(carnetBuscado);

            if (pos == -1)
            {
                Console.WriteLine("Estudiante no encontrado.");
            }
            else
            {
                Console.WriteLine("\n--- DATOS ENCONTRADOS ---");
                Console.WriteLine($"Carnet: {carnets[pos]} | Nombre: {nombres[pos]} | Género: {generos[pos]}");
                Console.WriteLine($"Facultad: {facultades[pos]} | Carrera: {carreras[pos]}");
                Console.WriteLine($"Materia: {nombresMaterias[pos]}");
                Console.WriteLine($"Notas Períodos: P1 = {notasPeriodos[pos, 0]:F2} | P2 = {notasPeriodos[pos, 1]:F2} | P3 = {notasPeriodos[pos, 2]:F2}");
                Console.WriteLine($"Asistencia Promedio: {asistenciasPromedio[pos]:F1}%");
                Console.WriteLine($"Promedio Final: {promediosFinales[pos]:F2}");
                Console.WriteLine($"Estado: {(aprobados[pos] ? "APROBADO" : "REPROBADO")}");
            }
        }

        // ==========================================
        // 4. MODIFICAR NOTAS Y ASISTENCIAS
        // ==========================================
        static void ModificarNotasAsistencias()
        {
            Console.Clear();
            Console.WriteLine("--- MODIFICAR NOTAS Y ASISTENCIAS ---");
            string carnetBuscado = LeerCarnetValido();

            int pos = BuscarPorCarnet(carnetBuscado);

            if (pos == -1)
            {
                Console.WriteLine("Estudiante no encontrado.");
                return;
            }

            int periodo = LeerEnteroValido("¿Qué período desea modificar? (1, 2 o 3): ", 1, 3) - 1;

            double nuevaNota = LeerDoubleValido($"Ingrese nueva nota para el Período {periodo + 1} (0-10): ", 0, 10);
            double nuevaAsistencia = LeerDoubleValido($"Ingrese nueva asistencia para el Período {periodo + 1} (0-100%): ", 0, 100);

            notasPeriodos[pos, periodo] = nuevaNota;
            asistencias[pos, periodo] = nuevaAsistencia;

            double sumaN = 0, sumaA = 0;
            for (int p = 0; p < 3; p++)
            {
                sumaN += notasPeriodos[pos, p];
                sumaA += asistencias[pos, p];
            }
            promediosFinales[pos] = sumaN / 3.0;
            asistenciasPromedio[pos] = sumaA / 3.0;
            aprobados[pos] = (promediosFinales[pos] >= 6.0) && (asistenciasPromedio[pos] >= 75.0);

            Console.WriteLine("\n¡Notas y asistencias actualizadas correctamente!");
        }

        // ==========================================
        // 5. ELIMINAR ESTUDIANTE
        // ==========================================
        static void EliminarEstudiante()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR ESTUDIANTE ---");
            string carnetBuscado = LeerCarnetValido();

            int pos = BuscarPorCarnet(carnetBuscado);

            if (pos == -1)
            {
                Console.WriteLine("Estudiante no encontrado.");
                return;
            }

            for (int i = pos; i < totalEstudiantes - 1; i++)
            {
                nombres[i] = nombres[i + 1];
                generos[i] = generos[i + 1];
                carnets[i] = carnets[i + 1];
                facultades[i] = facultades[i + 1];
                carreras[i] = carreras[i + 1];
                nombresMaterias[i] = nombresMaterias[i + 1];
                promediosFinales[i] = promediosFinales[i + 1];
                asistenciasPromedio[i] = asistenciasPromedio[i + 1];
                aprobados[i] = aprobados[i + 1];

                for (int p = 0; p < 3; p++)
                {
                    notasPeriodos[i, p] = notasPeriodos[i + 1, p];
                    asistencias[i, p] = asistencias[i + 1, p];
                }
            }

            totalEstudiantes--;
            Console.WriteLine("\n¡Estudiante eliminado con éxito!");
        }

        // ==========================================
        // 6. ESTADÍSTICAS GENERALES
        // ==========================================
        static void EstadisticasGenerales()
        {
            Console.Clear();
            Console.WriteLine("--- ESTADÍSTICAS GENERALES ---");

            if (totalEstudiantes == 0)
            {
                Console.WriteLine("No hay datos para mostrar.");
                return;
            }

            double sumaPromedios = 0;
            double notaMax = promediosFinales[0];
            double notaMin = promediosFinales[0];
            int cantAprobados = 0;

            for (int i = 0; i < totalEstudiantes; i++)
            {
                sumaPromedios += promediosFinales[i];
                if (promediosFinales[i] > notaMax) notaMax = promediosFinales[i];
                if (promediosFinales[i] < notaMin) notaMin = promediosFinales[i];
                if (aprobados[i]) cantAprobados++;
            }

            int cantReprobados = totalEstudiantes - cantAprobados;
            double porcAprobados = ((double)cantAprobados / totalEstudiantes) * 100;
            double porcReprobados = ((double)cantReprobados / totalEstudiantes) * 100;

            Console.WriteLine($"1. Promedio General de la Universidad: {sumaPromedios / totalEstudiantes:F2}");
            Console.WriteLine($"2. Nota más alta: {notaMax:F2} | Nota más baja: {notaMin:F2}");
            Console.WriteLine($"3. Cantidad de Aprobados: {cantAprobados} | Cantidad de Reprobados: {cantReprobados}");
            Console.WriteLine($"4. Porcentaje de Aprobados: {porcAprobados:F1}% | Porcentaje de Reprobados: {porcReprobados:F1}%");
        }

        // ==========================================
        // 7. TOP 3 ESTUDIANTES
        // ==========================================
        static void Top3Estudiantes()
        {
            Console.Clear();
            Console.WriteLine("--- TOP 3 ESTUDIANTES (POR UNIVERSIDAD) ---");

            if (totalEstudiantes == 0)
            {
                Console.WriteLine("No hay estudiantes registrados.");
                return;
            }

            int[] indices = new int[totalEstudiantes];
            for (int i = 0; i < totalEstudiantes; i++) indices[i] = i;

            for (int i = 0; i < totalEstudiantes - 1; i++)
            {
                for (int j = 0; j < totalEstudiantes - 1 - i; j++)
                {
                    if (promediosFinales[indices[j]] < promediosFinales[indices[j + 1]])
                    {
                        int temp = indices[j];
                        indices[j] = indices[j + 1];
                        indices[j + 1] = temp;
                    }
                }
            }

            int tope = totalEstudiantes < 3 ? totalEstudiantes : 3;
            for (int k = 0; k < tope; k++)
            {
                int idx = indices[k];
                Console.WriteLine($"#{k + 1}: {nombres[idx]} - Nota: {promediosFinales[idx]:F2} | Asistencia: {asistenciasPromedio[idx]:F1}% | Facultad: {facultades[idx]}");
            }
        }

        // ==========================================
        // 8. REPORTES
        // ==========================================
        static void Reportes()
        {
            Console.Clear();
            Console.WriteLine("--- REPORTES ---");

            if (totalEstudiantes == 0)
            {
                Console.WriteLine("No hay estudiantes registrados.");
                return;
            }

            double sumaM = 0, sumaF = 0;
            int cantM = 0, cantF = 0;

            for (int i = 0; i < totalEstudiantes; i++)
            {
                if (generos[i] == "M") { sumaM += promediosFinales[i]; cantM++; }
                else if (generos[i] == "F") { sumaF += promediosFinales[i]; cantF++; }
            }

            Console.WriteLine("1. Promedio por Género:");
            Console.WriteLine($"   - Hombres (M): {(cantM > 0 ? (sumaM / cantM).ToString("F2") : "Sin datos")}");
            Console.WriteLine($"   - Mujeres (F): {(cantF > 0 ? (sumaF / cantF).ToString("F2") : "Sin datos")}");

            int posMejor = 0, posPeor = 0;
            for (int i = 1; i < totalEstudiantes; i++)
            {
                if (promediosFinales[i] > promediosFinales[posMejor]) posMejor = i;
                if (promediosFinales[i] < promediosFinales[posPeor]) posPeor = i;
            }

            Console.WriteLine("\n2. Rendimiento Extremo:");
            Console.WriteLine($"   - Mejor Estudiante: {nombres[posMejor]} (Nota: {promediosFinales[posMejor]:F2})");
            Console.WriteLine($"   - Peor Estudiante: {nombres[posPeor]} (Nota: {promediosFinales[posPeor]:F2})");
        }

        // ==========================================
        // FUNCIONES DE SELECCIÓN DE FACULTAD Y CARRERA
        // ==========================================

        static void SeleccionarFacultadyCarrera(int i)
        {
            int fac = SeleccionarSoloFacultad();
            int car = SeleccionarSoloCarrera(fac);

            facultades[i] = ObtenerSiglaFacultad(fac);
            carreras[i] = ObtenerNombreCarrera(fac, car);
        }

        static int SeleccionarSoloFacultad()
        {
            Console.WriteLine("\nSeleccione la Facultad:");
            Console.WriteLine("1. Ciencias Empresariales CCE"); //[cite: 1]
            Console.WriteLine("2. Ingeniería y Arquitectura IYA"); //[cite: 1]
            Console.WriteLine("3. Ciencias y Humanidades CCH"); //[cite: 1]
            Console.WriteLine("4. Ciencias de la Salud CCS"); //[cite: 1]
            return LeerEnteroValido("Opción (1-4): ", 1, 4);
        }

        static int SeleccionarSoloCarrera(int fac)
        {
            Console.WriteLine("\nSeleccione Carrera:");
            switch (fac)
            {
                case 1:
                    Console.WriteLine("1. Lic. Administración de Empresas\n2. Lic. Contaduría Pública\n3. Lic. Mercadeo y Negocios Internacionales\n4. Lic. Gestión de Negocios Digitales\n5. Lic. Relaciones Internacionales y Comercio Exterior\n6. Lic. Gastronomía y Hostelería"); //[cite: 1]
                    return LeerEnteroValido("Opción (1-6): ", 1, 6);
                case 2:
                    Console.WriteLine("1. Ing. Química\n2. Ing. Mecánica\n3. Ing. Desarrollo de Software\n4. Ing. Telecomunicaciones y Redes\n5. Arquitectura\n6. Ing. Civil\n7. Ing. Sistemas Informáticos\n8. Ing. Agronómica\n9. Ing. Industrial\n10. Ing. Eléctrica"); //[cite: 1]
                    return LeerEnteroValido("Opción (1-10): ", 1, 10);
                case 3:
                    Console.WriteLine("1. Lic. Diseño Gráfico Publicitario\n2. Lic. Ciencias Jurídicas\n3. Lic. Periodismo y Comunicación Audiovisual\n4. Lic. Idioma Inglés\n5. Lic. Ciencias Religiosas"); //[cite: 1]
                    return LeerEnteroValido("Opción (1-5): ", 1, 5);
                case 4:
                    Console.WriteLine("1. Doctorado en Medicina\n2. Lic. Enfermería\n3. Técnico en Enfermería\n4. Lic. Nutrición y Dietética\n5. Lic. Química y Farmacia"); //[cite: 1]
                    return LeerEnteroValido("Opción (1-5): ", 1, 5);
                default:
                    return 1;
            }
        }

        static string ObtenerSiglaFacultad(int fac)
        {
            string[] sig = { "CCE", "IYA", "CCH", "CCS" };
            return sig[fac - 1];
        }

        static string ObtenerNombreCarrera(int fac, int car)
        {
            if (fac == 1)
            {
                string[] c = { "Lic. Administración de Empresas", "Lic. Contaduría Pública", "Lic. Mercadeo y Negocios Internacionales", "Lic. Gestión de Negocios Digitales", "Lic. Relaciones Internacionales y Comercio Exterior", "Lic. Gastronomía y Hostelería" }; //[cite: 1]
                return c[car - 1];
            }
            if (fac == 2)
            {
                string[] c = { "Ing. Química", "Ing. Mecánica", "Ing. Desarrollo de Software", "Ing. Telecomunicaciones y Redes", "Arquitectura", "Ing. Civil", "Ing. Sistemas Informáticos", "Ing. Agronómica", "Ing. Industrial", "Ing. Eléctrica" }; //[cite: 1]
                return c[car - 1];
            }
            if (fac == 3)
            {
                string[] c = { "Lic. Diseño Gráfico Publicitario", "Lic. Ciencias Jurídicas", "Lic. Periodismo y Comunicación Audiovisual", "Lic. Idioma Inglés", "Lic. Ciencias Religiosas" }; //[cite: 1]
                return c[car - 1];
            }
            if (fac == 4)
            {
                string[] c = { "Doctorado en Medicina", "Lic. Enfermería", "Técnico en Enfermería", "Lic. Nutrición y Dietética", "Lic. Química y Farmacia" }; //[cite: 1]
                return c[car - 1];
            }
            return "";
        }

        static int BuscarPorCarnet(string carnet)
        {
            for (int i = 0; i < totalEstudiantes; i++)
            {
                if (carnets[i].Equals(carnet, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        // ==========================================
        // FUNCIONES AUXILIARES DE VALIDACIÓN RIGUROSA
        // ==========================================

        // Solo letras y espacios
        static string LeerSoloTexto(string mensaje)
        {
            string entrada;
            do
            {
                Console.Write(mensaje);
                entrada = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(entrada))
                {
                    Console.WriteLine("Error: Este campo es obligatorio y no puede quedar vacío.");
                }
                else if (!Regex.IsMatch(entrada, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    Console.WriteLine("Error: El campo solo debe contener letras (no se permiten números ni símbolos).");
                    entrada = "";
                }
            } while (string.IsNullOrEmpty(entrada));

            return entrada;
        }

        // Texto que admite letras y números (ej. Nombres de materia)
        static string LeerTextoAlfanumerico(string mensaje)
        {
            string entrada;
            do
            {
                Console.Write(mensaje);
                entrada = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(entrada))
                {
                    Console.WriteLine("Error: Este campo es obligatorio y no puede quedar vacío.");
                }
                else if (!Regex.IsMatch(entrada, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    Console.WriteLine("Error: Solo se permiten letras, números y espacios.");
                    entrada = "";
                }
            } while (string.IsNullOrEmpty(entrada));

            return entrada;
        }

        // Formato obligatorio: 4 números + 2 letras (Ejemplo: 2025RM)
        static string LeerCarnetValido()
        {
            string carnet;
            do
            {
                Console.Write("Ingrese el carnet (Ejemplo: 2025RM): ");
                carnet = Console.ReadLine()?.Trim().ToUpper();

                if (string.IsNullOrEmpty(carnet))
                {
                    Console.WriteLine("Error: El carnet es obligatorio.");
                }
                else if (!Regex.IsMatch(carnet, @"^\d{4}[A-Z]{2}$"))
                {
                    Console.WriteLine("Error: Formato inválido. Debe constar de 4 dígitos (año) y 2 letras (iniciales). Ej: 2025RM");
                    carnet = "";
                }
            } while (string.IsNullOrEmpty(carnet));

            return carnet;
        }

        // Solo 'M' o 'F'
        static string LeerGeneroValido()
        {
            string gen;
            do
            {
                Console.Write("Ingrese Género (M/F): "); //[cite: 1]
                gen = Console.ReadLine()?.Trim().ToUpper();

                if (gen != "M" && gen != "F")
                {
                    Console.WriteLine("Error: Debe ingresar únicamente 'M' o 'F'.");
                }
            } while (gen != "M" && gen != "F");

            return gen;
        }

        // Solo números enteros dentro del rango especificado
        static int LeerEnteroValido(string mensaje, int min, int max)
        {
            int num;
            while (true)
            {
                Console.Write(mensaje);
                string entrada = Console.ReadLine()?.Trim();

                if (int.TryParse(entrada, out num) && num >= min && num <= max)
                    return num;

                Console.WriteLine($"Error: Debe ingresar un número entero válido entre {min} y {max}.");
            }
        }

        // Solo valores decimales válidos dentro del rango especificado
        static double LeerDoubleValido(string mensaje, double min, double max)
        {
            double num;
            while (true)
            {
                Console.Write(mensaje);
                string entrada = Console.ReadLine()?.Trim();

                if (double.TryParse(entrada, out num) && num >= min && num <= max)
                    return num;

                Console.WriteLine($"Error: Debe ingresar un número válido entre {min} y {max}.");
            }
        }
    }
}
