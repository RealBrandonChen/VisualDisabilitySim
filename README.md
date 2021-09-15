# A visual disability evaluation system to help identify glaucoma based on VR

|![Supermarket](Assets/Supermarket.gif)     |![Stair](Assets/Stair.gif)         |   ![CIty](Assets/City.gif)        |
| -----------                               | -----------                       | ---------                         |
| *Figure1. Shopping in the supermarket*    | *Figure2. Climbing the stairs*    |*Figure3. Walking in the street*   |

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

    
#### This repository provides an introdcution to the visual evaluation system and part of the code base for the paper *"Use of Virtual Reality Simulation to Identify Vision-Related Disability in Patients With Glaucoma"*.
#### JAMA Ophthalmology: [paper link](https://jamanetwork.com/journals/jamaophthalmology/fullarticle/2762850)
#### Patent: Leung CK, Lam AKN, To E. Visual Disability Detection System Using Virtual Reality (U.S. Patent No.US20170273552A1).
#### [Video Link](https://edhub.ama-assn.org/jn-learning/video-player/18315135)

# Licenese and Citation
#### This project is licensed under the terms of the MIT license. By using the software, you are agreeing to the terms of the [license agreement](LICENSE).

#### If you use this code in your research, please cite us as follows:
```
@article{10.1001/jamaophthalmol.2020.0392,
    author = {Lam, Alexander K. N. and To, Elaine and Weinreb, Robert N. and Yu, Marco and Mak, Heather and Lai, Gilda and Chiu, Vivian and Wu, Ken and Zhang, Xiujuan and Cheng, Timothy P. H. and Guo, Philip Yawen and Leung, Christopher K. S.},
    title = "{Use of Virtual Reality Simulation to Identify Vision-Related Disability in Patients With Glaucoma}",
    journal = {JAMA Ophthalmology},
    volume = {138},
    number = {5},
    pages = {490-498},
    year = {2020},
    issn = {2168-6165},
    doi = {10.1001/jamaophthalmol.2020.0392},
    url = {https://doi.org/10.1001/jamaophthalmol.2020.0392},
}
```
# Recommended System
> #### Operating system: 64-bit Windows 7, 64-bit Windows 8 (8.1) or 64-bit Windows 10
> #### CPU: CPU Core i5-2500K 3.3GHz / AMD CPU Phenom II X4 940
> #### Memory: 8 GB RAM
> #### GPU: Nvidia GPU GeForce GTX 1070 / AMD GPU Radeon Radeon RX590
> #### Storage space: 5 GB free space required

# Getting Started
- ### Project introduction
> The visual disability evaluation system is a virtual reality based simulator that provides three daily life environments: supermarket shopping, stair climbing and street walking, which returns the testing result between the patients with glaucoma and healthy individuals.
- ### Hardware and software setup:
> #### HTC VIVE/VIVE Pro Headset/Oculus + controller (Xbox All Series / PlayStation 4) hardware connection
> #### Steam & SteamVR ([HTC VIVE Setup Tutorial](https://support.steampowered.com/steamvr/HTC_Vive/)) + PlayStation4 controller driver ([DS4Windows](https://ryochan7.github.io/ds4windows-site/))
- ### Download the [release demo](https://github.com/RealBrandonChen/VisualDisabilitySim/releases/download/Compiled/Integrated_VR_Project_Build.7z), unzip and click on it.
> This is the demo of virtual supermarket scene, you will experience some situations of daily supermarket shopping, such as buying drinks, snacks, toys and so on.
- ### Data analysis
> Your testing result is stored in `AppPath/TestResult/Program/`. You can download and start this [post-processing system](https://github.com/RealBrandonChen/VisualDisabilitySim/releases/download/Compiled/VR.Visual.Disability.Performance.Scoring.rar) to generate a visual disability score with reference to the Mahalanobis distance (like the image below). Please note that this image is only for illustration and inadequate for official diagnostic.
> 
><Image of the evaluation score src="Assets/Eva.png" width="400" height="350">

# Contributing and Cooperation
#### 
#### This project is led by the [Virtual Reality Lab](https://www.ophthalmology.hku.hk/virtualreality), Department of Ophthalmology, HKU LKS Faculty of Med. Any contributions or suggestions are welcomed. Please contact the research team for more detailed information and we are open for the cooperation.
