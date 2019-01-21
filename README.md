Wersja polska
# Cel projektu
Stworzyć oprogramowanie dla firmy zajmującej się dyspozycją kontenerów i wysyłką ich za pomocą statków. Program ma na podstawie przygotowanych danych zwracać informację, jakie jest najbardziej optymalne ułożenie kontenerów na poszczególnych statkach i planować wysyłkę kontenerów.
# Działanie programu
1. Dane generowane są losowo do plików .txt, można wygenerować trzy statki o rozmiarach od 50 do 100 w każdym wymiarze oraz dowolną liczbę kontenerów o rozmiarach z przedziału 1-40 w każdym wymiarze. Wysokość kontenerów, mimo że losowa, jest stała dla każdego kontenera. Dane oznaczone są timestampem
2. Istnieje możliwość dodania nowego statku - wtedy najstarszy statek (ten o najmniejszym timestamp) jest usuwany.
3. Kontenery podzielone są na dostawy (mają różne timestampy, ale jeden timestamp może być przypisany do wielu kontenerów). Kontenery z niższym timestampem mają pierwszeństwo wysyłki
4. Zaimplementowano 2 algorytmy układania kontenerów na statkach - program sprawdza, któy algorytm jest korzystniejszy i go wybiera do stworzenia dostawy
5. Wynikiem działania programu jest raport w postaci interfejsu graficznego przedstawiającego jak powinny zostać ułożone kontenery, na jakim statku i którą dostawą mają płynąć
# Jak korzystać z programu
1. W lewym górnym rogu znajduje się przycisk do generowania danych. Generuje on 3 losowe statki oraz 3 sety kontenerów zawierające kolejno 100, 60 i 30 obiektów
2. Po prawej stronie u góry znajduje się przycisk dodawanie statku. Po jego naciśnięciu do systemu dodawany jest losowo wygenerowany statek
3. Obok jest przycisk do tworzenia dostaw. Po jego naciśnięciu, dane są pobierane z plików, a następnie program układa kontenery na statkach w najbardziej optymalny sposób dzieląc wszystko na poszczególne dostawy
4. Po stworzeniu dostaw pojawiają się listy rozwijalne. Jedna służy do wyboru dostawy (znajduje się tu też informacja jakim statkiem dostawa będzie realizowana). Druga lista służy do wyboru poziomów w obrębie danej dostawy i danego statku
5. Po wygenerowaniu dostaw na środku znajduje się rysunek przedstawiający rozłożenie kontenerów na wybranym poziomie statku z wybranej dostawy
6. Na dole po prawej stronie jest przycisk do zakończenia pracy z aplikacją
