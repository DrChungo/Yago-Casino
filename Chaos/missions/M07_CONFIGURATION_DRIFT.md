# M07 – Deriva de configuración

El sistema ahora es coherente.
El modelo tiene sentido.
Los endpoints hacen lo que deben.

Y aun así… **no siempre se comporta igual**.

Lo ejecutas en un sitio y funciona de una manera.
Lo ejecutas en otro y funciona distinto.

No hay errores evidentes.
No hay excepciones claras.
Pero algo no cuadra.

---

## Contexto

A lo largo del tiempo, el proyecto ha incorporado:

- configuraciones por entorno
- valores por defecto
- ajustes “rápidos” para desarrollo
- decisiones locales que no se revisaron globalmente

Nadie lo hizo mal.
Simplemente, **la configuración ha ido derivando**.

El resultado es un sistema que:
- funciona
- pero **no siempre igual**

---

## Tu objetivo

Detectar y corregir los efectos de la deriva de configuración para que el sistema sea **predecible y coherente**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El comportamiento es consistente entre ejecuciones razonables
- ✅ La configuración existente se respeta
- ✅ No se elimina funcionalidad

No se trata de unificarlo todo.
Se trata de **entender qué depende de la configuración y por qué**.

---

## Reglas importantes

- ❌ No borres archivos de configuración
- ❌ No ignores valores “porque solo fallan a veces”
- ❌ No fuerces valores en código para que coincidan
- ❌ No asumas que Development es el único entorno

- ✅ Asume que la configuración tiene intención
- ✅ Detecta inconsistencias reales
- ✅ Alinea comportamiento y configuración

---

## Qué puede estar pasando (orientativo)

Algunas situaciones posibles:

- Valores distintos según entorno que afectan a la lógica
- Propiedades configurables usadas de forma incoherente
- Valores por defecto distintos en distintos puntos
- Configuración que se lee pero no siempre se usa
- Cambios de comportamiento silenciosos según el arranque

Nada de esto es un bug “clásico”.
Es **comportamiento emergente**.

---

## Cómo afrontar esta misión

1. Revisa la configuración existente
2. Observa cómo influye en el comportamiento
3. Detecta diferencias entre ejecuciones
4. Decide:
   - qué debe variar según entorno
   - qué NO debería variar
   - qué valores deben estar alineados

Después:
- ajusta la lógica
- refuerza la coherencia
- deja claro en el código cuándo algo depende de configuración

---

## Concepto clave

La configuración **también es código**.

Si no se entiende,
el sistema deja de ser predecible.

Este ejercicio simula una situación muy común:
> “Funciona… pero no siempre igual. Y nadie sabe por qué.”

Ahora te toca a ti.

---

## Entrega

Al finalizar:

1. El sistema debe comportarse de forma consistente
2. Las diferencias por configuración deben tener sentido
3. El comportamiento debe poder explicarse
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué deriva detectaste
- explica cómo la corregiste

---

## Aviso final

Los errores de configuración:
- no rompen el build
- no saltan rápido
- son los más caros de detectar

Aprende a identificarlos aquí,
no en producción.