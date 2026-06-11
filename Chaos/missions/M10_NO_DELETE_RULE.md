# M10 – No se puede borrar

El sistema ha crecido.
Tiene más código.
Tiene más lógica.
Tiene más decisiones acumuladas.

Ahora, al mirarlo, es posible que pienses:
> “Esto sobra.”  
> “Si quito esto, todo sería más simple.”  
> “Esto nunca debió existir.”

Puede que incluso tengas razón.

Pero en esta misión hay una regla clara:

👉 **No se puede borrar.**

---

## Contexto

El proyecto ha pasado por muchas manos.
Cada parte que existe:
- fue escrita por alguien
- tenía un propósito en su momento
- fue integrada en algún punto

Puede que ahora:
- algunas piezas parezcan innecesarias
- haya solapamientos
- existan contradicciones aparentes
- el sistema sea difícil de entender

Aun así, **todo lo que existe forma parte del sistema**.

---

## Tu objetivo

Conseguir que **todo el sistema funcione correctamente** manteniendo **toda la funcionalidad existente**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ Todo el código existente sigue presente
- ✅ El comportamiento es coherente
- ✅ El sistema se puede explicar

No se trata de limpiar.
Se trata de **hacer convivir lo que hay**.

---

## Regla principal (no negociable)

❌ **No se puede eliminar código existente.**

Esto incluye:
- controllers
- endpoints
- propiedades del modelo
- lógica previa
- configuraciones añadidas
- comportamiento existente

Si algo parece sobrar:
- hay que integrarlo
- adaptarlo
- o hacer que su propósito sea claro

Eliminarlo **no es una solución válida**.

---

## Otras reglas importantes

- ❌ No comentes código para “desactivarlo”
- ❌ No encapsules todo para ocultarlo
- ❌ No neutralices comportamientos con valores fijos
- ❌ No conviertas todo en opcional para evitar decidir

- ✅ Asume que todo tiene sentido para alguien
- ✅ Armoniza comportamientos
- ✅ Haz explícitas las relaciones entre partes

---

## Qué puede estar ocurriendo

Es posible que encuentres situaciones como:

- Dos partes del sistema haciendo cosas similares
- Lógica que parece duplicada pero no es exactamente igual
- Comportamientos que se pisan
- Información que parece redundante
- Decisiones que compiten entre sí

Nada de esto está “mal” por sí solo.
El problema es **cómo conviven**.

---

## Cómo afrontar esta misión

1. Identifica todo lo que existe
2. Intenta entender por qué podría ser necesario
3. Decide cómo debe comportarse el sistema cuando todo está activo
4. Ajusta la lógica para que:
   - no haya contradicciones
   - no se pierda información
   - el resultado sea coherente

Esto puede implicar:
- coordinar flujos
- priorizar comportamientos
- hacer explícitas dependencias
- documentar decisiones mediante código

---

## Concepto clave

Eliminar código es fácil.
**Integrar decisiones heredadas es difícil.**

Esta misión existe para entrenar exactamente eso.

En un entorno real, muchas veces:
> No puedes borrar porque alguien depende de ello.

Aprende a trabajar bajo esa condición.

---

## Entrega

Cuando termines:

1. El sistema debe funcionar correctamente
2. Ninguna funcionalidad existente ha sido eliminada
3. El comportamiento debe ser coherente
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué partes parecían eliminarse
- explica cómo las integraste en el sistema

---

## Aviso final

Esta misión no se supera “rápido”.
Se supera cuando aceptas que:

> Mantener sistemas reales no va de limpiar,
> va de **convivir con decisiones heredadas**.
