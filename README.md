# Neuro-Myo-PCA

This project demonstrates the usage of the Myo armband and Principe component analysis (PCA) algorithm. When Myo armband is placed on forearm, program can detect various hand gestures from trained data based on muscle movements. The main flaw of this approach is that this way when Myo armband is moved, user has to recalibrate program data. The whole project is made for a better understanding of how does the PCA algorithm works, and how to use it for dimensionality-reduction on a practical problem. The project is using [Myo armband's API](https://support.getmyo.com/hc/en-us) and [Accord.NET Framework](http://accord-framework.net/).

| ![image_1.png](https://github.com/igradeca/Neuro-Myo-PCA/blob/master/image_1.png) | 
|:--:| 
| *Figure shows program UI where data from eight sensors are represented on graphs with its EMG and root mean square (RMS) values. The ninth graph shows orientation data.* |

| ![image_3.jpg](https://github.com/igradeca/Neuro-Myo-PCA/blob/master/image_3.jpg) | 
|:--:| 
| *Figure shows a window for adding new movements after Myo armband is connected and placed on forearm.* |

| ![image_4.jpg](https://github.com/igradeca/Neuro-Myo-PCA/blob/master/image_4.jpg) | 
|:--:| 
| *Figure shows UI for calculation of principal components from recorded data, representation of eigenvalues energies and drawing its graph.* |

After connecting the Myo, data can be fetched and saved for later processing and PCA calculation. PCA algorithm results are represented as eigenvectors and eigenvalues. Only three eigenvalues are enough because they contain about 90% of its total energy.

| ![image_5.jpg](https://github.com/igradeca/Neuro-Myo-PCA/blob/master/image_5.jpg) | 
|:--:| 
| *3D graph of recorded hand movement from three most important eigenvalues* |

The image above shows a 3D graph of eigenvalues when a particular movement was made. The movement will make a data cluster in someplace on the graph and to detect a movement in real-time, by comparing the distance between real-time fetched location with clusters middle one. The one with minimal value will be considered as the right one.

This project is made for educational and academic purposes.
