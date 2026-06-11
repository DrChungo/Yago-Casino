# M08 – Dependencia del orden

El sistema parece estable.
La configuración parece clara.
Los endpoints hacen lo que deben.

Y aun así, algo extraño ocurre:

👉 **Cambiar el orden del código cambia el comportamiento.**

No hay errores de compilación.
No hay excepciones evidentes.
Pero pequeñas variaciones producen resultados distintos.

---

## Contexto

Con el tiempo, el proyecto ha ido acumulando:

- lógica distribuida
- inicializaciones implícitas
- supuestos sobre qué ocurre antes y después
- código que depende de valores ya calculados

Nada de esto es visible a primera vista.
Nada “parece mal”.

Pero el sistema **depende del orden**.

---

## Tu objetivo

Detectar y corregir las dependencias implícitas del orden para que el sistema sea **determinista y razonable**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El comportamiento no depende del orden accidental
- ✅ Las decisiones explícitas se respetan
- ✅ No se elimina funcionalidad

El sistema debe comportarse igual siempre que:
> *las condiciones sean las mismas.*

---

## Reglas importantes

- ❌ No congeles valores “porque así funciona”
- ❌ No dupliques lógica solo para evitar el problema
- ❌ No muevas código al azar hasta que “parezca estable”
- ❌ No ignores el problema porque “ahora ya va bien”

- ✅ Asume que el orden actual tiene efectos reales
- ✅ Entiende por qué ese orden importa
- ✅ Decide qué orden tiene sentido y refuérzalo

---

## Qué puede estar ocurriendo (orientativo)

Algunas situaciones posibles:

- Código que depende de valores inicializados antes
- Propiedades que se modifican varias veces
- Inicialización parcial usada prematuramente
- Lógica que asume efectos secundarios previos
- Cambios de comportamiento al mover bloques de código

Nada de esto es un bug evidente.
Es **comportamiento dependiente del flujo**.

---

## Cómo afrontar esta misión

1. Analiza el flujo de ejecución del programa
2. Identifica qué código depende de qué
3. Observa qué cambia al alterar el orden
4. Decide:
   - qué debe ocurrir primero
   - qué depende de ese primer paso
   - qué no debería depender del orden

Después:
- refuerza dependencias explícitas
- elimina dependencias accidentales
- haz el flujo comprensible

---

## Concepto clave

Un sistema que depende del orden sin decirlo es un sistema frágil.

Hoy funciona.
Mañana, un pequeño cambio lo rompe todo.

Este ejercicio simula algo muy común:
> “Si mueves esto de sitio… deja de ir.  
> Nadie sabe por qué.”

Ahora te toca eliminar esa fragilidad.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse de forma consistente
2. El orden del código no debe cambiar el resultado
3. El flujo debe poder explicarse
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué dependencia de orden encontraste
- explica cómo la corregiste

---

## Aviso final

Este tipo de problema:
- no se detecta con tests simples
- no aparece siempre
- suele llegar a producción

Aprende a verlo aquí,
donde puedes equivocarte sin coste.