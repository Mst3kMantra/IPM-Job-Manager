   M S B A M L   `   `   `  ���� J  FIPM Job Manager, Version=1.1.4.5, Culture=neutral, PublicKeyToken=null9!clr-namespace:IPM_Job_Manager_netIPM_Job_Manager_net  .    (IPM_Job_Manager_net.FirstTimeSetupWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:IPM_Job_Manager_net       � Title$  FirstTimeSetupWindow��$	��237��$	��502�� � WindowStartupLocation$ CenterOwner=�.���� $��1q� �� CornerRadius$ 8��
�������$��1q�����  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FF2D2D2D�� � Offset$
 0.85Q�� $ 	#FF353535��$ 0.597Q�� $ 	#FF1D1D1D��$ 0.107Q�� $ 	#FF2D2D2D��$
 0.31Q�._�� $��10q� � RowDefinitions �� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��500* �.+��� $��0��$��0,65,0,-126q�$��1��.+��� $��1,1,1,0q�
�������._��� 	 �� Orientation$	 
Horizontal=�.+���    JobTitle#��[  $��1��$��Data Directory���� -   
 ��Name$
 txtPath��$��10,0,0,0q�$	��290��$��20���� -    ��Name$ btnFindFolder��$��15,0,0,0q�$��30��.��...�� 
�������$	��1pxq�._��� $	 
Horizontal=�.+��� #��[  $��1��$��	Demo Mode���� -    ��Name$ chkDemoMode��$��0,0,0,0q�$��Center=��� -   $ 	btnFinish��$	��100��$��40��.��Finish