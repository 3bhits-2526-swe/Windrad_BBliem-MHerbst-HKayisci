# 🌪️ Windrad – Virtual Engineering System

Visualisierung der Aerodynamik von Objekten und des Wirkungsgrades von Windrädern durch dynamische CFD-Simulation und Wake-Effekt-Analyse.

---

## 📋 Konzept

**Ein Satz:** Mehrere Windräder erzeugen ein dynamisches Windfeld, in dem die Aerodynamik eines ausgewählten 3D-Objekts analysiert wird – beide Strömungsverhalten und Wake-Effekt werden gleichzeitig sichtbar.

---

## 🎮 Features

| Input | Verarbeitung | Output |
|-------|--------------|--------|
| 3D-Objekt auswählen | Strömungsvektoren berechnen | Aerodynamik-Visualisierung |
| Windgeschwindigkeit (Slider) | Wake-Effekt simulieren | Rotationsgeschwindigkeit |
| Windrad-Position/Abstand | Verlustfaktor zwischen Windrädern | Wirkungsgrad-Anzeige |
| Start/Stop-Steuerung | Dynamische Windanpassung | Strömungsvisualisierung (Vektoren/Partikel) |

---

## 🕹️ Interaktion (Teacher of Things)

- **Objekt auswählen** → Aerodynamik beobachten
- **Windgeschwindigkeit anpassen** (Slider 0–100) → Live-Effekte sehen
- **Windräder ein/ausschalten** → Wake-Effekt verstehen
- **QR-Code-Import** → Custom 3D-Objekte (Skylander-Modus) (eventuell)
- **Ventilator-Feedback** → Wie viel Wind beim letzten Rad ankommt (eventuell)

---

## ⚙️ Technisch

### Rendering & Engine
- **URP** (Universal Render Pipeline) – Performance & Shader Graph Support
- **Shader Graph** für Drag-Visualisierung (Color-Mapping)
- 32-Bit Index Format für hochpoly CFD-Meshes (>65.535 Vertices)

---

## 📐 Regeln (Virtual Engineering)

✓ Keine Windgeschwindigkeit → Keine Rotation, keine Aerodynamik-Anzeige  
✓ Objekt außerhalb Windbereich → Keine Strömungsvisualisierung  
✓ Windrad gestoppt → Dahinterliegende Windräder bekommen mehr Wind (weniger Verlust)

---

## 🎯 Virtual Engineering-Anteil

- ☑ **Anzeige** (Dashboard / Unity)
- ☑ **Simulation** (Strömung & Wake-Effekt)
- ☐ Digital Twin
- ☐ Live-Daten
- ☐ Fehler­erkennung

---

## 👥 Team

Bliem, Herbst, Hasan Kayisci

---

## 📊 Visualisierung

```
Wirkungsgrad: 100% → 80% → 60%
        ↓
    [Windrad 1] —→ [Windrad 2] —→ [Windrad 3]
         |               |               |
       Wind          Wake-Zone       Objekt (farbcodiert)
```
