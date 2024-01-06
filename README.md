# Programming Digital Twins - Client Framework Components
This is the source repository for client framework software components (written primarily in C#) related to my Digital Twins Programming course at Northeastern University. The intent of this repository is to provide students with a baseline client framework library that provides core integration functionality for the digital twin application components (which are housed in a separate repository). For convenience to the reader, much of the basic functionality has already been implemented (such as configuration logic, consts, interfaces, and test cases).

These classes and their relationships respresent a notional design that aligns with the requirements listed in [Programming Digital Twins Requirements](https://github.com/orgs/programming-digital-twins/projects/1). These requirements encapsulate the programming exercises presented in my course [Buliding Digital Twins](TBD).

## Links, Exercises, Updates, Errata, and Clarifications

Please see the following links to access exercises for this project. Please note that many of the exercises and sample source code in this repository is based on the Constrained Device Application design and exercises from my book, [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/).
 - [Programming Digital Twins Requirements](https://github.com/orgs/programming-digital-twins/projects/1)
 - [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/)

## How to use this repository
If you're reading [Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401), you'll see a partial tie-in with the exercises described in some of the chapters and lab modules exercises. While Programming the IoT exercises are mostly Python and Java, this repository is implemented in C#; it does, however, share some of the same functionality as the Programming the IoT Gateway Device App (GDA), but without the application wrapper and state machine component.

## This repository aligns to exercises in Programming Digital Twins, and partially to Programming the Internet of Things
These components are all written in C#, and have been partially tested using .NET 4.1 on Windows 11. It is intended that this library be built into a dll and then linked into the Digital Twin App (housed in a separate repository).

## How to navigate the directory structure for this repository
This repository is comprised of the following top level paths:
- [PdtCfwComponents](https://github.com/programming-digital-twins/pdt-cfw-components/tree/alpha/PdtCfwComponents): Contains the following source trees:
  - LabBenchStudios/Src/Main/ProgrammingDigitalTwins
    - [Common](https://github.com/programming-digital-twins/pdt-cfw-components/tree/master/PdtCfwComponents/LabBenchStudios/Src/Main/ProgrammingDigitalTwins/Common): Contains shared components.
    - [Connection](https://github.com/programming-digital-twins/pdt-cfw-components/tree/master/PdtCfwComponents/LabBenchStudios/Src/Main/ProgrammingDigitalTwins/Connection): Contains integration-related components (e.g., MQTT, persistence, etc.).
    - [Data](https://github.com/programming-digital-twins/pdt-cfw-components/tree/master/PdtCfwComponents/LabBenchStudios/Src/Main/ProgrammingDigitalTwins/Data): Contains data serialization / deserialization / translation components.
This repository is comprised of the following top level paths:
- [PdtCfwComponents.Tests](https://github.com/programming-digital-twins/pdt-cfw-components/tree/alpha/PdtCfwComponents.Tests): Contains the following test (source) trees:
  - LabBenchStudios/Src/Test/ProgrammingDigitalTwins
    - [Connection](https://github.com/programming-digital-twins/pdt-cfw-components/tree/master/PdtCfwComponents.Tests/LabBenchStudios/Src/Main/ProgrammingDigitalTwins/Connection): Contains unit / integration tests for the integration-related components (e.g., MQTT, persistence, etc.).
    - [Data](https://github.com/programming-digital-twins/pdt-cfw-components/tree/master/PdtCfwComponents.Tests/LabBenchStudios/Src/Main/ProgrammingDigitalTwins/Data): Contains unit tests for the data serialization / deserialization / translation components.

Here are some other files at the top level that are important to review:
- [PdtCfwComponents.sln](https://github.com/programming-digital-twins/pdt-cfw-components/blob/alpha/PdtCfwComponents.sln): The Visual Studio solution file.
- [README.md](https://github.com/programming-digital-twins/pdt-cfw-components/blob/alpha/README.md): This README.
- [LICENSE](https://github.com/programming-digital-twins/pdt-cfw-components/blob/alpha/LICENSE): The repository's LICENSE file.

Lastly, here are some 'dot' ('.{filename}') files pertaining to dev environment setup that you may find useful (or not - if so, just delete them after cloning the repo):
- [.gitignore](https://github.com/programming-digital-twins/pdt-cfw-components/blob/alpha/.gitignore): The obligatory .gitignore that you should probably keep in place, with any additions that are relevant for your own cloned instance.

NOTE: The directory structure and all files are subject to change based on feedback I receive from readers of my blog and students in my Building Digital Twins class, as well as improvements I find to be helpful for overall repo betterment.

# Other things to know

## Pull requests
PR's are disabled while the codebase is being developed.

## Updates
Much of this repository, and in particular unit and integration tests, will continue to evolve, so please check back regularly for potential updates. Please note that API changes can - and likely will - occur at any time.

# REFERENCES
This repository has external dependencies on other open source projects. I'm grateful to the open source community and authors / maintainers of the following libraries:

Core components / exercises:

- [DTDLParser](https://github.com/digitaltwinconsortium/DTDLParser)
  - Reference: Digital Twin Consortium and contributors. (2024) [Online]. Available: https://github.com/digitaltwinconsortium/DTDLParser.
- [InfluxDB.Client](https://github.com/influxdata/influxdb-client-csharp)
  - Reference: InfluxData, Inc. Influx DB C# Client. (2024) [Online]. Available: https://github.com/influxdata/influxdb-client-csharp.
- [MQTTnet](https://github.com/dotnet/MQTTnet)
  - Reference: .NET Foundation and contributors. MQTTnet .NET library for MQTT communications. (2024) [Online]. Available: https://github.com/dotnet/MQTTnet.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
  - Reference: James Newton-King. Json.NET JSON framework for .NET. (2024) [Online]. Available: https://github.com/JamesNK/Newtonsoft.Json.
- [NUnit](https://nunit.org/)
  - Reference: Charlie Poole, Rob Prouse. Nunit unit testing framework for .NET languages. (2024) [Online]. Available: https://github.com/nunit.

NOTE: This list will be updated as others are incorporated.

# FAQ
For typical questions (and answers) to the repositories of the Programming the IoT project, please see the [FAQ](https://github.com/programming-the-iot/book-exercise-tasks/blob/default/FAQ.md).

# IMPORTANT NOTES
This code base is under active development.

If any code samples or other technology this work contains, describes, and / or is subject to open source licenses or the intellectual property rights of others, it is your responsibility to ensure that your use thereof complies with such licenses and/or rights.

# LICENSE
Please see [LICENSE](https://github.com/programming-digital-twins/pdt-edge-components/blob/default/LICENSE) if you plan to use this code.
