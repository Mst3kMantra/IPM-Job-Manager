   M S B A M L   `   `   `  ����J  FIPM Job Manager, Version=1.1.4.5, Culture=neutral, PublicKeyToken=null9!clr-namespace:IPM_Job_Manager_netIPM_Job_Manager_net  5   	   *    $IPM_Job_Manager_net.OperationsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:IPM_Job_Manager_net     5   	   -   5          � Title$  Operations Editor��5   	   $	��650��6#   $	��500��6/    � WindowStartupLocation$ CenterOwner=�5	   	    � SizeToContent$ Height=�5
   	   .��5      �� $��1q�6    �� CornerRadius$ 8��6!   
�������$��1q�6F   ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�63   .��5      �  � Color$ 	#FF2D2D2D��6    � Offset$
 0.85Q�61   � 5      $ 	#FF353535��6   $ 0.597Q�61   � 5      $ 	#FF1D1D1D��6   $ 0.107Q�61   � 5      $ 	#FF2D2D2D��6   $
 0.31Q�61   5      ._�5   
   � $��10q�6    � RowDefinitions 5      �� 5      $
��auto �6    �� 5      $��* �6    	 � ColumnDefinitions	 5      �� 5      $
��400* �6#   �� 5      $
��100* �6#   .+�5      �� $��5q�6   .+�5!      ��    JobTitle#��[  6   $��0��6:   $
��10pxq�6J   $��0,0,0,0q�6Y   $	��Top=�6j   .��6�   Operations Editor6�   �� 5"      -   5        
 ��Name$
 txtOperations��5"   6    �� 	IsEnabled . $	��150��6M   �� 5#      $��2q�6   
�������$ 2��6A   ._�5$      �  5%      �� 5&      $
��auto �6,   �� 5'      $
��auto �6,   	 5)      �� 5*      $
��150* �6/   �� 5+      $
��100* �6/   .+�5.      �� $��0��6!   $��0,0,1,0q�61   
�������6K   �� 5/      $��1��6!   $��1,0,0,0q�61   
�������6K   �� 50      $��Center=�6    #��[  6=   .��6k   
Operations6w   �� 51      $��1��6    $��Center=�60   #��[  6M   .��6{   
Assigned6�   �� 52      -   5         ��Name$ lstOperations��52   "   $��0��6{   $��1��6�   ��6�   �� *OperationList+��53      �� 54   "   .R�55   &   �� ��60   �� 56   #   58      �� 59      -   5        $ lstAssigned��59   ?   $��1��6"   $��1��62   ��6�   �� *AssignedEmployeeList+��5:      �� 5;   "   .R�5<   &   �� ��60   �� 5=   #   5?      5@      5A      5B      �� 5C      $��5,5,5,5q�6   $��1��6*   .+�5D      �� #��[  6   $��0��6:   $��2��6J   $
��10pxq�6^   $��0,0,0,0q�6m   $	��Top=�6~   .��6�   	Employees6�   �� 5E      -   5        $ lstEmployees��5E      ��6q   �� *
UserList+��5F      �� 5G      .R�5H      �� ��6(   ��  �� Path$ Username�6J   5I      5K      �� 5L      -   5         ��Name$ btnAddOperation��5L       �� 	IsDefault . $��0,5,0,0q�6a   .��6r   Add Operation6�   �� 5M      -   5        $ btnRemoveOperation��5M      $��0,5,0,0q�6V   .��6g   Remove Operation6y   �� 5N      -   5        $ btnAssignOperation��5N      $��0,5,0,0q�6V   .��6g   Assign Operation6y   �� 5O      -	   5        $ btnUnassignOperation��5O      $��0,5,0,0q�6Z   .��6k   Unassign Operation6   5P      5Q      5R      5S      