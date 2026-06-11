# Reglas del Chaos Training

Este documento define las reglas del entrenamiento.
No son sugerencias.
No son recomendaciones.
No se negocian durante las misiones.

Estas reglas existen para que el entrenamiento funcione
y para simular condiciones reales de trabajo.

---

## Regla 0 – El caos es intencional

Si algo parece mal diseñado, confuso o incoherente:
- NO es un error del ejercicio
- NO es un fallo del proyecto
- NO es un descuido

👉 Forma parte del entrenamiento.

El objetivo es aprender a trabajar en esas condiciones.

---

## Regla 1 – Todo lo que existe tiene un motivo

Si algo está en el código, se asume que:

- alguien lo escribió con intención
- alguien dependió de ello
- alguien pensó que era correcto

Aunque no te guste.
Aunque lo veas mal.
Aunque lo habrías hecho distinto.

❌ No existe el concepto de “esto es basura”.
✅ Existe el concepto de “esto existe y hay que entenderlo”.

---

## Regla 2 – No borrar para que funcione

Está **prohibido**:

- borrar controllers
- borrar endpoints
- borrar propiedades del modelo
- borrar lógica existente
- comentar código para “desactivarlo”
- revertir todo para volver a un estado cómodo

Eliminar funcionalidad **NO es una solución válida**.

Si algo no encaja:
- hay que integrarlo
- adaptarlo
- o hacer explícita su intención

---

## Regla 3 – Nadie arregla lo suyo

- No integras tus propios cambios
- No corriges directamente tus propios errores
- No decides en solitario el impacto de tu trabajo

Este entrenamiento simula trabajo en equipo real:
> lo que tú haces lo sufre otra persona

---

## Regla 4 – No preguntar intenciones

Está prohibido:

- preguntar “qué querías hacer”
- explicar a otro por qué hiciste algo
- coordinar decisiones fuera del código
- justificar cambios verbalmente

El único lenguaje válido es:
- el código
- la estructura
- el comportamiento
- los commits
- el post‑mortem

---

## Regla 5 – Git es obligatorio, pero no protagonista

Debes usar Git para:
- trabajar en ramas
- integrar cambios
- resolver conflictos
- mantener historia

Pero Git:
- NO es la excusa
- NO es el objetivo principal
- NO justifica malas decisiones

> “Era complicado en Git” no es una justificación suficiente.

---

## Regla 6 – No reiniciar, no empezar de cero

Está prohibido:

- crear un repositorio nuevo
- copiar el código a otro sitio y volverlo a pegar
- borrar la historia
- reconstruir desde el baseline

Trabajas **con lo que hay**, no con lo que te gustaría tener.

---

## Regla 7 – El sistema debe funcionar siempre

En todo momento, al final de cada misión:

- ✅ El proyecto debe compilar
- ✅ `/health` debe responder
- ✅ `/status` debe responder

Un sistema que “ya lo arreglarán luego”
no es válido en este entrenamiento.

---

## Regla 8 – Asumir sin certeza es parte del trabajo

No siempre sabrás:
- cuál era la decisión correcta
- qué se pretendía exactamente
- cuál es la mejor solución

Aun así:
👉 tendrás que decidir

Tomar decisiones razonables
es parte del entrenamiento.

No decidir también es una decisión.

---

## Regla 9 – El post‑mortem es obligatorio

Después de cada misión:

- debes completar el POST‑MORTEM
- con honestidad
- sin justificarte
- sin copiar de otros

El post‑mortem:
- no se corrige
- no se evalúa por “acierto”
- se evalúa por **razonamiento**

Sin post‑mortem, la misión no existe.

---

## Regla 10 – No actuar por reflejo

Si algo no funciona:

❌ No borres
❌ No reinicies
❌ No pruebes cosas al azar
❌ No “toques hasta que pase”

✅ Para
✅ Lee
✅ Piensa
✅ Decide con criterio
✅ Luego actúa

Este entrenamiento castiga el pánico
y premia la comprensión.

---

## Regla final – Responsabilidad

Este proyecto no busca que:
- escribas código perfecto
- aciertes siempre
- lo hagas rápido

Busca que:
- no rompas sistemas por reflejo
- sepas convivir con decisiones ajenas
- entiendas consecuencias
- pienses como parte de un equipo

Si algo sale mal pero sabes explicar por qué,
estás aprendiendo.

Si algo funciona pero no sabes explicar cómo,
todavía no.

---

## Recordatorio

Esto no es un curso.
No es un examen.
No es un juego.

Es un entrenamiento real
para aprender a trabajar con sistemas reales.

Y los sistemas reales:
no son limpios,
no son coherentes,
no son amables.

Aprende a habitarlos.