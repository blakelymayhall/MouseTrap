# This is a script created to remove garbage from the 
# start of file names downloaded from the MIT course 
# intro to C++

# Cannot work with path on iCloud drive, so move temporarily to this folder

import sys
import os

# Get argument from input for the parent directory 
parent_dir_name = str(sys.argv[1])

# Change dir to the input arg
os.chdir(parent_dir_name)

# Get subdirs, remove any hidden files
subdir_list = os.listdir(os.getcwd())
subdir_list = [i for i in subdir_list if "." not in i]

for subdir in subdir_list:
    os.chdir(subdir)
    file_list = os.listdir(os.getcwd())
    for file in file_list:
        if file.find("BCM") < 0:
            os.rename(file, "BCM_"+file[(file.find("_")+1):])

        if file.find("BCM_MIT6_096IAP11_") >= 0:
            os.rename(file, file.replace("BCM_MIT6_096IAP11_",""))
    os.chdir('../')
