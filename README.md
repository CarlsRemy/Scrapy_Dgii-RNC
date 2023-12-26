# Web Scraper DGII (República Dominicana) - Consulta de RNC

Este repositorio contiene una herramienta de consola desarrollada en C# que realiza web scraping a la página de la Dirección General de Impuestos Internos (DGII) de la República Dominicana. La aplicación permite realizar consultas de Registro Nacional de Contribuyentes (RNC) de manera eficiente y obtener la información asociada.


## Características Principales:

- Validación Robusta de RNC: La herramienta realiza una validación rigurosa del parámetro de RNC proporcionado antes de iniciar la consulta a la DGII. La validación incluye:

	- Longitud Exacta: Se verifica que el RNC tenga la longitud correcta, evitando errores que podrían surgir al proporcionar un número incompleto.
	- Formato Numérico: Se garantiza que el RNC esté compuesto exclusivamente por caracteres numéricos, excluyendo cualquier otro carácter como guiones, espacios u otros símbolos que podrían provocar errores en la búsqueda.

- Web Scraping Eficiente: Implementado con PuppeteerSharp, esta herramienta aprovecha la potencia de Puppeteer para realizar web scraping eficiente en sitios web dinámicos, ofreciendo soluciones especialmente valiosas para lenguajes como PHP, que carecen de soluciones nativas para esta tarea.

- Consulta de RNC: Proporciona una interfaz de consola interactiva que permite a los usuarios ingresar un RNC y obtener la información asociada de manera rápida y precisa.

- Datos Estructurados: La herramienta devuelve la información del RNC en formato [JSON](#estructura-de-la-respuesta), lo que facilita la integración con aplicaciones y sistemas que pueden consumir datos estructurados. Esto es especialmente útil para entornos que manejan datos en formato JSON, como PHP.

- Fácil de Usar: La interfaz de consola ha sido diseñada pensando en la usabilidad, guiando al usuario de manera sencilla a través del proceso de consulta, lo que hace que la herramienta sea accesible incluso para aquellos con poca experiencia técnica.

- Esta aplicación es ideal para entornos donde las soluciones nativas para web scraping son limitadas, y el uso de PuppeteerSharp ofrece una alternativa poderosa para obtener datos de manera eficiente desde sitios web dinámicos.

## Respuesta

En caso de que se produzca un error de formato o falta del parámetro RNC, la respuesta será un string (UTF-8) que proporciona una breve descripción de la razón del error. En caso contrario, se devuelve un objeto serializado.

#### Estructura de la Respuesta:
``` json
[{
	"RNC":"401-05261-1",
	"Razon_Social":"UNIVERSIDAD TECNOLOGICA DE SANTIAGO UTESA",
	"Nombre":"",
	"Categoria":"",
	"Pagos":"NORMAL",
	"Estado":"ACTIVO",
	"Actividad":"ENSEÑANZA UNIVERSITARIA EXCEPTO FORMACIÓN DE POSGRADO",
	"Administracion":"ADM LOCAL OGC SANTIAGO"
}]
```

### Uso:

- Clona el repositorio en tu máquina local.
- Compila la aplicación de consola en tu entorno de desarrollo preferido.
- Executa la aplicacion resultante por medio de tu lenguaje de preferencia, recordando que para hacer esto deber indicar la ruta del .exe de la app seguido de parametro

``` php
$RNC = isset($_GET("RNC")) ? $_GET("RNC") : "";
$RNC = preg_replace("/[^0-9]/", "", $RNC);

if ((strlen($RNC) != 9 && strlen($rnc) != 11)) {
	header_remove();
	http_response_code(400);
	header('Content-Type: application/json; charset=utf-8');
	echo json_encode(["error"=>"Parametro Incorrecto","message"=>"El RNC no es válido (longitud incorrecta)."]);
	return;
}

$executablePath = "C:\\Program Files (x86)\\DGII\\Scrapy_Dgii-RNC.exe";

// Crear el comando completo
$command = "$executablePath $RNC";
// Ejecutar el programa de C# desde PHP
$output = shell_exec($command);
// Mostrar la salida en la página web
$array = json_decode($output, true);

if ($array !== null && json_last_error() !== JSON_ERROR_NONE) {
	// Manejar el error de decodificación
	header_remove();
	http_response_code(200);
	header('Content-Type: application/json; charset=utf-8');
	echo json_encode($array);
} else {

	header_remove();
	
	if (is_string($output)) {
		http_response_code(400);
		header('Content-Type: application/json; charset=utf-8');
		echo ["error"=>"Parametro Incorrecto","message"=>$output];
		return;
	}

	http_response_code(200);
	header('Content-Type: application/json; charset=utf-8');
	echo $output;
}
```


## Requisitos:

> [!IMPORTANT]
>  .NET Core SDK instalado en el entorno de ejecución.

### Contribuciones:
¡Las contribuciones son bienvenidas! Si encuentras mejoras o correcciones, no dudes en hacer un fork y enviar un pull request.

#### **Aviso Legal:**

> [!WARNING]
> Esta aplicación se desarrolla con fines educativos y no se garantiza la precisión ni la disponibilidad continua de la información obtenida de la DGII. Se recomienda utilizar esta herramienta de manera ética y respetar los términos de servicio y limitaciones del sitio web de la DGII.
