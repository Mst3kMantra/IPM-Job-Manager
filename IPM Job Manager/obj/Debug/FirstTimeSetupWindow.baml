   M S B A M L   `   `   `  ����J  FIPM Job Manager, Version=1.1.4.5, Culture=neutral, PublicKeyToken=null9!clr-namespace:IPM_Job_Manager_netIPM_Job_Manager_net  5   	   .    (IPM_Job_Manager_net.FirstTimeSetupWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:IPM_Job_Manager_net     5   	     � Title$  FirstTimeSetupWindow��5   	   $	��237��6&   $	��502��63    � WindowStartupLocation$ CenterOwner=�5	   	   .��5      �� $��1q�6    �� CornerRadius$ 8��6!   
�������$��1q�6F   ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�63   .��5      �  � Color$ 	#FF2D2D2D��6    � Offset$
 0.85Q�61   � 5      $ 	#FF353535��6   $ 0.597Q�61   � 5      $ 	#FF1D1D1D��6   $ 0.107Q�61   � 5      $ 	#FF2D2D2D��6   $
 0.31Q�61   5      ._�5   
   � $��10q�6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6     � ColumnDefinitions 5      �� 5      $
��500* �6#   .+�5!      �� $��0��6   $��0,65,0,-126q�6)   $��1��6>   .+�5"      �� $��1,1,1,0q�6   
�������._�5#      �� 	 �� Orientation$	 
Horizontal=�6!   .+�5$      ��    JobTitle#��[  6    $��1��6B   $��Data Directory��6V   �� 5%      -   5        
 ��Name$
 txtPath��5%   "   $��10,0,0,0q�63   $	��290��6E   $��20��6Q   �� 5&      -   5         ��Name$ btnFindFolder��5&   !   $��15,0,0,0q�6T   $��30��6f   .��6q   ...6v   5'      5(      �� 5)      
�������$	��1pxq�6-   ._�5*      �� $	 
Horizontal=�6!   .+�5+      �� #��[  6    $��1��6B   $��	Demo Mode��6V   �� 5,      -   5         ��Name$ chkDemoMode��5,   #   $��0,0,0,0q�68   $��Center=�6I   5-      5.      �� 5/      -   5        $ 	btnFinish��5/   (   $	��100��6   $��40��6S   .��6_   Finish6g   50      52      53      54      