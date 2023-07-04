# -*- coding: utf-8 -*-
"""
Created on Tue Jul  4 13:44:38 2023

@author: frede
"""
import os
import os.path


def save_first_n_characters(file_path, n, output_file_path):
    with open(file_path, 'r') as file:
        content = file.read()
        first_n_chars = content[:n]

    with open(output_file_path, 'w') as output_file:
        output_file.write(first_n_chars)

# Example usage:
path = os.getcwd()
#print(path)
os.chdir(path+"/ConsoleApp/Data")
#os.chdir("C:\Users\frede\OneDrive\Skrivebord\MasterThesis\StringIndexing\ConsoleApp\Data")
path = os.getcwd()
#print(path)

input_file_path = path+'\english_33554432.txt'
output_file_path = path+'\english_256.txt'
n = 256  # Replace with the desired number of characters

sizes = [256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 32768*2, 32768*4, 262144, 524288, 1048576,
         2097152, 4194304, 8388608, 16777216, 33554432]


# =============================================================================
# for size in sizes:
#     n = size
#     input_file_path = path+'\english_33554432.txt'
#     output_file_path = path+'\english_'+str(size)+'.txt'
#     save_first_n_characters(input_file_path, n, output_file_path)
#     
# for size in sizes:
#     n = size
#     input_file_path = path+'\DNA_33554432.txt'
#     output_file_path = path+'\DNA_'+str(size)+'.txt'
#     save_first_n_characters(input_file_path, n, output_file_path)
#     
# for size in sizes:
#     n = size
#     input_file_path = path+'\proteins_33554432.txt'
#     output_file_path = path+'\proteins_'+str(size)+'.txt'
#     save_first_n_characters(input_file_path, n, output_file_path)
# =============================================================================
    
for size in sizes:
    n = size
    input_file_path = path+'\\realDNA_33554432.txt'
    output_file_path = path+'\\realDNA_'+str(size)+'.txt'
    save_first_n_characters(input_file_path, n, output_file_path)
    
    
    
    
#save_first_n_characters(input_file_path, n, output_file_path)
