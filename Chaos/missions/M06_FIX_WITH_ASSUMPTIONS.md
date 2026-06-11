# M06 – Arreglar asumiendo cosas

Hasta ahora, has identificado problemas de significado.
Has visto que el sistema expresa ideas contradictorias.

Ahora viene la parte más incómoda:

👉 **Nadie te va a decir qué era lo correcto.**
👉 **Nadie dejó decisiones escritas.**
👉 **Y aun así, hay que arreglarlo.**

---

## Contexto

El sistema ha evolucionado de forma no coordinada.
Hay decisiones implícitas en el código, pero no están explicadas.
Algunas cosas parecen asumir reglas que nadie escribió.

Puede que:
- el modelo sugiera una cosa
- los endpoints sugieran otra
- la configuración sugiera otra distinta

No hay documentación.
No hay reuniones pasadas.
No hay “la persona que lo hizo”.

Solo hay código.

---

## Tu objetivo

Hacer que el sistema funcione de forma coherente **asumiendo decisiones implícitas**, cumpliendo todas estas condiciones:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El sistema sigue haciendo todo lo que hacía antes
- ✅ El comportamiento ahora es consistente
- ✅ Las decisiones asumidas quedan reflejadas en el código

No se trata de adivinar “la verdad”.
Se trata de **asumir una verdad razonable y hacer que el sistema la cumpla**.

---

## Reglas muy importantes

- ❌ No elimines funcionalidades “porque confunden”
- ❌ No conviertas todo en opcional para no decidir
- ❌ No ignores campos o lógica existentes
- ❌ No pongas `TODO` como solución

- ✅ Asume que el sistema quería hacer algo concreto
- ✅ Toma decisiones explícitas
- ✅ Ajusta el comportamiento para cumplir esas decisiones

---

## Qué significa “asumir”

En esta misión, asumir significa:

- Decidir qué representa realmente un estado
- Decidir qué datos son obligatorios y cuáles no
- Decidir cómo debe comportarse el sistema ante inconsistencias
- Decidir qué interpretación tiene más sentido dado el código existente

Estas decisiones **no están escritas en ningún sitio**.
Pero el código da pistas.

---

## Cómo trabajar en esta misión

1. Analiza el comportamiento actual del sistema
2. Identifica qué cosas parecen asumidas pero no garantizadas
3. Decide una interpretación coherente
4. Refuerza esa interpretación en el código:
   - validaciones
   - valores por defecto con sentido
   - lógica coherente
   - alineación entre partes del sistema

El objetivo no es “hacer que pase”.
Es **hacer que tenga sentido**.

---

## Concepto clave

En sistemas reales, muchas decisiones:
- nunca se documentan
- se transmiten solo a través del código
- sobreviven a las personas que las tomaron

Este ejercicio simula exactamente eso.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse de forma consistente
2. Las decisiones asumidas deben notarse en el funcionamiento
3. El código debe reflejar esas decisiones
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué asumiste
- explica por qué te pareció razonable

---

## Aviso final

Aquí no se evalúa si “acertaste”.
Se evalúa si:

- pensaste
- asumiste responsabilidad
- dejaste el sistema mejor de lo que lo encontraste

En proyectos reales, esto es el día a día.