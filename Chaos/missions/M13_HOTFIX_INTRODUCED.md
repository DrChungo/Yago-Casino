# M13 – Hotfix urgente

Algo no funciona como se esperaba.
No está claro cuándo empezó.
No está claro por qué.

Pero hay presión para solucionarlo **ya**.

Alguien actuó rápido.
El problema aparente desapareció.
El sistema volvió a responder.

Ahora te toca a ti trabajar **con las consecuencias**.

---

## Contexto

En algún momento reciente:

- se detectó un problema
- alguien aplicó un cambio rápido
- el sistema volvió a “funcionar”
- no hubo tiempo para revisar efectos secundarios

El hotfix:
- cumplió su objetivo inmediato
- no se integró con calma
- no se pensó a largo plazo

Esto no es una excepción.
Es lo normal en sistemas reales.

---

## Tu objetivo

Identificar los efectos colaterales del hotfix
y **reintegrar el sistema de forma coherente**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El problema urgente sigue solucionado
- ✅ El sistema no introduce nuevas incoherencias
- ✅ No se elimina funcionalidad previa

El hotfix **no se puede quitar**.
Hay que convivir con él.

---

## Reglas importantes

- ❌ No elimines el hotfix “porque es feo”
- ❌ No lo reviertas sin entenderlo
- ❌ No ignores efectos secundarios
- ❌ No limpies ocultando comportamientos

- ✅ Asume que el hotfix resolvió algo real
- ✅ Entiende qué cambiaron exactamente
- ✅ Integra esa decisión en el sistema

---

## Qué suele pasar con un hotfix (orientativo)

Pueden aparecer situaciones como:

- Lógica especial añadida sin integrarse con el resto
- Comportamientos distintos según el flujo
- Suposiciones rotas en otras partes del sistema
- Condiciones añadidas solo para “salir del paso”
- Déficits de coherencia tras el arreglo rápido

Nada de esto es accidental.
Es **el precio de la urgencia**.

---

## Cómo afrontar esta misión

1. Identifica claramente qué resolvía el hotfix
2. Analiza cómo lo resolvía
3. Observa qué rompe o deja incoherente
4. Decide:
   - cómo integrar esa solución en el sistema
   - qué reglas deben actualizarse
   - qué partes ahora necesitan adaptarse

Después:
- consolida el comportamiento
- elimina contradicciones
- deja el sistema en un estado razonable

El objetivo no es criticar el hotfix.
Es **hacerlo sostenible**.

---

## Concepto clave

Los hotfixes:
- salvan el día
- comprometen la semana siguiente
- y, si nadie los integra bien, arruinan meses

Este ejercicio simula algo constante en la vida real:
> “Esto no estaba pensado así… pero ahora existe.”

Aprende a trabajar con ello.

---

## Entrega

Cuando termines:

1. El hotfix sigue activo y funcional
2. El sistema es coherente con él
3. No se han introducido nuevos problemas
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué resolvía el hotfix
- explica qué problemas introducía
- explica cómo lo integraste correctamente

---

## Aviso final

En proyectos reales, la mayoría de los problemas graves
no vienen del diseño original,
sino de **hotfixes no digeridos**.

Aprende a absorberlos aquí,
antes de heredar veinte en producción.