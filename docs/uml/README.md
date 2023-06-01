# C4 Diagrams

## Introduction

The service visualises its software architecture using the [C4 model](https://c4model.com/). We have used [PlantUML](https://plantuml.com/) to generate the [images](images) from the `.puml` files in this folder. PlantUML and the C4 model are combined using [C4-PlantUML](https://github.com/plantuml-stdlib/C4-PlantUML) which provides diagram types to render C4 model components.

These are enriched with sprites from the [plantuml-icon-font-sprites](https://github.com/tupadr3/plantuml-icon-font-sprites) repo which provides fontawesome and devicons sprites for PlantUML.

## Creating Diagrams

### Prerequisits

PlantUML is distributed as a .jar file. You therefore need to [install the Java runtime](https://www.java.com/en/download/)

Once that is installed, download the [plantuml.jar](http://sourceforge.net/projects/plantuml/files/plantuml.jar/download) file and save it to the parent folder of this repository.

You can also install a [VS Code extension for PlantUML](https://github.com/plantuml-stdlib/C4-PlantUML#snippets-for-visual-studio-code) to aid writing the UML.

### Generating Diagram Images

Open a command prompt or terminal. Change to the parent directory of this repo and run the following command

```
java -jar .\plantuml.jar .\find-a-tuition-partner\docs\uml\ -o images
```

This will override any existing diagram images in the images folder.
