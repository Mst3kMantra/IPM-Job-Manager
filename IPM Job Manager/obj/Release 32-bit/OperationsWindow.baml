   M S B A M L   `   `   `  ���� J  FIPM Job Manager, Version=1.0.4.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:IPM_Job_Manager_netIPM_Job_Manager_net  *    $IPM_Job_Manager_net.OperationsWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3local!clr-namespace:IPM_Job_Manager_net     -     � Title$  Operations Editor��$	��400��$	��650�� � WindowStartupLocation$ CenterOwner=�.��� $��10q� � RowDefinitions �� $
��auto ��� $��* � � ColumnDefinitions �� $
��400* ��� $
��100* �.+��� $��5q�
�������.+���    JobTitle#��[  $��0��$
��10pxq�$��0,0,0,0q�
�������$	��Top=�.��Operations Editor�� -    ��Name$ txtOperations�� �� 	IsEnabled . $	��150���  �� $
��auto ��� $��* � �� $
��150* ��� $
��100* �.+��� $��Center=�#��[  .��
Operations�� $��1��$��Center=�#��[  .��
Assigned�� -    ��Name$ lstOperations��$	��100��$��0��$��1������ *OperationList+���� .R��� ���� �� -   $ lstAssigned��$��1��$��1������ *AssignedEmployeeList+���� .R��� ���� �� $��5,5,5,5q�$��1��.+��� #��[  $��0��$��2��$
��10pxq�$��0,0,0,0q�
�������$	��Top=�.��	Employees�� -   $ lstEmployees������ *
UserList+���� .R��� ����  �� Path$ Username��� -    ��Name$ btnAddOperation��$��0,5,0,0q�.��Add Operation�� -   $ btnRemoveOperation��$��0,5,0,0q�.��Remove Operation�� -   $ btnAssignOperation��$��0,5,0,0q�.��Assign Operation�� -	   $ btnUnassignOperation��$��0,5,0,0q�.��Unassign Operation