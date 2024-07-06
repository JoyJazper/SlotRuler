# slotruler

**slotruler** is a modular slot game designed to provide control over timing and results effortlessly.

Branch Submission will never be merged to Master.

## Getting Started

### Prerequisites

**Unity Version - 2022.3.24f1**
**- WebGL - resolution ( 960x600 ) -**

Ensure you have the following packages installed in your Unity project:

- DoTween (free) - version: 5.6.7
- Text Mesh Pro - version: 3.0.6

## User Guide

**UI Elements**:
- The current UI is for demonstration purposes and is not intended for commercial use.
- The back button is located at the top right corner of the target selection page.
- Some buttons may not look like traditional buttons.

## Optimization Required:

1. **2 Canvas (Static / Dynamic)**: Separate canvases into static and dynamic elements for better performance.
2. **Resolution**: Ensure UI anchors are adaptable to various resolutions to avoid stretching.
3. **Data Structures**: Optimize by replacing heavy data types like lists and dictionaries where possible.
4. **Performance Testing**: While tested at various frame rates, additional QA is recommended for robust performance.
5. **Modular Testing**: Implement more tests for specific modules like movement to ensure reliability.
6. **Design Patterns**: Consider employing more advanced design patterns for better scalability and maintainability.
7. **Class Segmentation**: Divide classes like movement manager into smaller, more manageable segments for setup, roll control, and state management.
8. **Texture Mapping**: for 2D elements for reducing.

## Improvements

1. **Sound**
2. **Win Effect**

## Contributing

Contributions are welcome! Please submit a pull request or open an issue to discuss any changes or improvements.

## Important Note

- UI elements sourced from online resources are for non-commercial use only.

---

Feel free to customize this further according to your specific needs and project details.
