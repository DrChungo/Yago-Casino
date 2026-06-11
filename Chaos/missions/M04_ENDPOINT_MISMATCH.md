# M04 – Los endpoints ya no encajan

El modelo ha cambiado.
El sistema ha evolucionado.
Ahora aparece un problema distinto:

Los **endpoints no reflejan correctamente lo que el sistema es capaz de hacer**.

No es que estén rotos.
Es que **dicen una cosa y hacen otra**.

---

## Contexto

El proyecto ha pasado por varias manos.
El modelo de datos ha evolucionado.
Algunos endpoints:
- asumen un modelo antiguo
- devuelven información parcial
- no representan correctamente el estado actual

Aun así:
- compilan
- responden
- parecen funcionar

Este es el tipo de error más peligroso en un sistema real.

---

## Tu objetivo

Hacer que los endpoints representen **fielmente** el estado actual del sistema.

Al finalizar esta misión:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ Los datos devueltos tienen sentido con el modelo actual
- ✅ El comportamiento de cada endpoint es coherente
- ✅ No se ha eliminado funcionalidad existente

No se trata de volver atrás.
Se trata de **alinear el contrato del sistema con su realidad**.

---

## Reglas clave

- ❌ No borres endpoints para “simplificar”
- ❌ No ignores propiedades del modelo
- ❌ No asumas que "si responde 200 está bien"
- ❌ No preguntes qué hacía antes este endpoint

- ✅ Asume que alguien confía en estas rutas
- ✅ Asume que los datos devueltos importan
- ✅ Ajusta el comportamiento para que sea honesto

---

## Situaciones posibles (no exhaustivas)

Puede ocurrir que:

- Un endpoint no devuelva todos los datos relevantes
- La respuesta sea contradictoria con el modelo
- El nombre del endpoint ya no describa bien lo que hace
- La lógica interna no tenga sentido con el estado actual del sistema
- Dos endpoints expresen la misma idea de forma distinta

Nada de esto es accidental.
Simplemente **ya no están alineados**.

---

## Cómo afrontar esta misión

1. Revisa qué endpoints existen
2. Analiza qué datos devuelven y por qué
3. Compáralos con el modelo actual
4. Decide:
   - qué debe representar cada endpoint
   - qué información es obligatoria
   - qué comportamientos son incoherentes

Después:
- ajusta el código
- mantén la funcionalidad
- haz que lo que se expone sea veraz y consistente

---

## Concepto importante

Un endpoint es un **contrato**.

Si el sistema ha cambiado pero el contrato no,
el problema **no es de comunicación**, es de diseño.

Este ejercicio no busca perfección.
Busca **honestidad en el comportamiento del sistema**.

---

## Entrega

Cuando termines:

1. El proyecto debe arrancar correctamente
2. Los endpoints deben ser coherentes con el sistema
3. El comportamiento debe poder explicarse
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué incoherencias detectaste
- explica qué decisiones tomaste y por qué

---

## Aviso final

A partir de este punto:

- compilar ya no garantiza nada
- que algo “responda” no significa que esté bien

Empieza a pensar en términos de:
> *¿Esto expresa realmente lo que el sistema es ahora?*