# M14 – Funciona… en algunos sitios

El sistema ahora funciona.
Al menos, a veces.

Hay casos en los que:
- responde bien
- los datos parecen correctos
- nadie se queja

Y hay otros casos en los que:
- el comportamiento cambia
- los resultados no encajan
- nadie entiende por qué

Este es uno de los peores estados posibles para un sistema.

---

## Contexto

Después de los últimos cambios y hotfixes:

- el sistema **no está completamente roto**
- tampoco está completamente bien
- su comportamiento depende del contexto

Puede que:
- funcione en algunos escenarios
- falle en otros
- dependa de datos, orden, entorno o combinación de factores

Esto no genera errores claros.
Genera confusión.

---

## Tu objetivo

Hacer que el sistema **funcione correctamente en todos los escenarios razonables**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El comportamiento es consistente
- ✅ No hay “casos especiales inexplicables”
- ✅ No se elimina funcionalidad existente

No se trata de elegir un caso y optimizarlo.
Se trata de **hacer que todos tengan sentido**.

---

## Reglas importantes

- ❌ No ignores los casos que fallan
- ❌ No soluciones el problema “dejando solo el caso que va”
- ❌ No introduzcas excepciones silenciosas
- ❌ No hagas que los datos se adapten al error

- ✅ Asume que todos los escenarios son válidos
- ✅ Encuentra el patrón común
- ✅ Ajusta el sistema para cubrirlo correctamente

---

## Qué puede estar ocurriendo (orientativo)

Algunos ejemplos típicos:

- Datos válidos que solo fallan en ciertas combinaciones
- Comportamientos distintos según el origen de la petición
- Condiciones que solo se cumplen a veces
- Lógica que depende de supuestos no garantizados
- Efectos secundarios acumulativos

Nada de esto aparece siempre.
Y eso es lo peligroso.

---

## Cómo afrontar esta misión

1. Observa cuándo funciona bien el sistema
2. Observa cuándo no
3. Compara ambos escenarios
4. Pregúntate:
   - ¿qué condición cambia?
   - ¿qué suposición deja de cumplirse?
   - ¿por qué el sistema no lo protege?

Después:
- refuerza las garantías
- elimina comportamientos “a medias”
- deja el sistema predecible

---

## Concepto clave

Un sistema que funciona solo a veces:
- es difícil de depurar
- genera desconfianza
- consume tiempo y energía

Este ejercicio simula una situación muy común:
> “A mí me va.”  
> “Pues a mí no.”  
> “No sabemos por qué.”

Ahora te toca eliminar esa incertidumbre.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse igual en todos los escenarios esperables
2. Los casos antes defectuosos deben estar cubiertos
3. El comportamiento debe ser razonable y explicable
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica en qué escenarios fallaba
- explica por qué
- explica cómo lo resolviste

---

## Aviso final

Los bugs intermitentes:
- no se reproducen fácilmente
- desgastan equipos
- suelen llegar a producción

Aprende a eliminarlos aquí,
donde fallar no tiene coste.
