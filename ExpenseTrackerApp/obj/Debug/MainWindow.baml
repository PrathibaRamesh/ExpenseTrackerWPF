   M S B A M L   `   `   `  ����  ExpenseTrackerApp"    ExpenseTrackerApp.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  Expense Tracker��5   	   $	��550��6!   $	��830��6.    � WindowStartupLocation$ CenterScreen=�6:    � WindowStyle$ SingleBorderWindow=�5   	   
������ .��5	      � .+�5
   
   �� -   5         ��Name$ myTabControl��5
   s   ���#2C3E50
����������#3498DB$��1q�6S   $��10q�6g   .@�5      �� $��Record Expenses��6   $��231,1,-247,-1q�6/   .��5       � -   5          �Name$ addDataFrame��5        � Source$% Frontend\RecordExpensesTab.xaml?�6.   5      �� 5      $��View Expenses��6   $��251,1,-267,-1q�6-    J� RenderTransformOrigin$ 0.558,0.075&�6D   .��5       � -   5        $ viewDataFrame��5      $# Frontend\ViewExpensesTab.xaml?�6/   5      �� 5      -   5         ��Name$ tabItem3��5   E   $��Monthly Report��6   $��271,1,-287,-1q�6.   .��5       � -   5        $ reportDataFrame��5      $$ Frontend\MonthlyReportTab.xaml?�61   5       � 	Resources 5      	 �� 
TargetType
 �� Property �� Value �� 
TargetType �� CornerRadius ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  '��       '�� �     �� 5      "	 ��6   .�5      �� 
 � �� �#2C3E5063   �� 5      
 � �� �#3498DB64   �� 5      
 � ��$ 1q�68   �� 5      
 � ��
 �����63   5       �� 5#      "	 ��6   .�5$      �� 
 � �� �#2C3E5063   �� 5%      
 � ��
 �����63   �� 5&      
 � �� �#3498DB64   �� 5'      
 � ��$ 0,0,0,2q�68   �� 5(      
 � ��$ 0,0,5,0q�6/   �� 5)      
 � ��$
 10,5q�60   �� 5*      
 � ��$ 30��62   �� 5+      
 � ��$ 14(�61   �� 5,      
 � �� 5-      �� 5.      " ��6.   .R�5/   "   � .+�50   &   �� $��Border��6-   #��z��6;   #��z��6g   $ 5,5,0,0��6�   #��z��6�   ._�51   *   �� $ ContentSite��6;   $��Center=�6P   $��Center=�6k   $��Header��6�   ��. 6�   52   '   53   #    54   "   P� 55   &    �@��
IsSelected . . �56   *   �� $ Border��61   
 � �� �#3498DB6[   �� 57   *   $ ContentSite��61   
 � ��
 �����6l   58   '   P� 59   &    �@��
IsSelected .  . �5:   *   �� $ Border��61   
 � �� �#2C3E506[   5;   '   5=      5?      5@      5A      5B      5C      5D      