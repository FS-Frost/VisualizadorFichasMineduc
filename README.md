# VisualizadorFichasMineduc
Aplicación para obtener y guardar fichas de colegios desde el Mineduc.

# Requisitos
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472).
- [Visual Studio IDE](https://visualstudio.microsoft.com/es/vs/) 2013 o superior.

# Estructura
- MineducRbd: Librería (.dll) para obtener datos.
- MineducRbdViewer: Interfaz gráfica en WPF.

# Compilación
1. Abrir **VisualizadorFichasMineduc.sln** en Visual Studio IDE.
2. Seleccionar solución **VisualizadorFichasMineduc** y compilarla.
3. Obtener en **VisualizadorFichasMineduc\MineducRbdViewer\bin\Debug** el ejecutable con sus dependencias.

# Modo de uso
1. Crear archivo CSV y colocar en una columna llamada "RBD" los RBD de los colegios a obtener.
2. Abrir "MineducRbdViewer.exe".
3. Presionar el botón "Cargar RBD" y seleccionar el archivo CSV con los RBD.
4. Esperar a que se obtengan todas las fichas. El programa notificará cuando acabe el procesamiento.
   Para cancelar la obtención de fichas, presionar el botón "Cancelar".
5. Una vez obtenidas las fichas, presionar el botón "Guardar" para guardar los datos en formato CSV.
