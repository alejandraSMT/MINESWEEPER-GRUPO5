# https://rosettacode.org/wiki/Minesweeper_game#Ruby
# https://stackoverflow.com/questions/1414951/how-do-i-get-elapsed-time-in-milliseconds-in-ruby (time)
# https://stackoverflow.com/questions/60979729/how-to-cast-float-to-integer 
# https://makeitrealcamp.gitbook.io/ruby-book/manipulacion-de-archivos 
# https://stackoverflow.com/questions/3518329/add-a-new-line-in-file
# https://coderwall.com/p/ssdcua/bubble-sort-in-ruby
# https://www.geeksforgeeks.org/ruby-integer-to_f-function-with-example/

PCT = 0.15
# definir 8x8 el tablero
WIDTH = 8
HEIGHT = 8
# cálculo aleatorio de minas
NUM_MINES = (WIDTH * HEIGHT * PCT).round

# ordenar los tiempos y nombres de los jugadores (extraidos de .txt)
def bubble_sort(times,names)
  n = times.length
  swapped = true
  while swapped do
    swapped = false
    (n-1).times do |i|
      if times[i] > times[i + 1]
        times[i], times[i + 1] = times[i + 1], times[i]
        names[i], names[i + 1] = names[i + 1], names[i]
        swapped = true
      end
    end
  end
end

def time_diff_milli(start, finish)
    (finish - start) * 1000.0
end

# función de creación aleatoria de minas
def create_mines(sx, sy)
  # llena el arreglo de false
  arr = Array.new(WIDTH) { Array.new(HEIGHT, false) }
  NUM_MINES.times do
    x, y = rand(WIDTH), rand(HEIGHT)
    redo if arr[x][y] or (x == sx and y == sy)
    arr[x][y] = true
  end
  arr
end

def num_marks 
  $screen.inject(0) { |sum, row| sum + row.count("?") }
end

# mostrar el tablero
def show_grid revealed = false
  if revealed
    puts "1 2 3 4 5 6 7 8"
    puts $mines.transpose.map { |row| row.map { |cell| cell ? "*" : " " }.join(" ") }
  else
    puts "Hay #{NUM_MINES} minas, #{num_marks} banderas marcadas."
    puts "1 2 3 4 5 6 7 8"
    puts $screen.transpose.map{ |row| row.join(" ")}
  end
end

SURROUND = [-1,0,1].product([-1,0,1]) - [[0,0]]
def surrounding x, y
  SURROUND.each do |dx, dy|
    yield(x+dx, y+dy) if (0...WIDTH).cover?(x+dx) and (0...HEIGHT).cover?(y+dy)
  end
end

def clear_space x, y
  return unless $screen[x][y] == "."
  count = 0
  surrounding(x, y) { |px, py| count += 1 if $mines[px][py] }
  if count == 0
    $screen[x][y] = " "
    surrounding(x, y) { |px, py| clear_space px, py }
  else
    $screen[x][y] = count.to_s
  end
end

# funcion que evalúa si el jugador ya marcó todas las minas
def victory? 
  return false if $mines.nil?
  return false if num_marks != NUM_MINES
  mines_left = NUM_MINES
  WIDTH.times do |x|
    HEIGHT.times do |y|
      mines_left -= 1 if $mines[x][y] and $screen[x][y] == "?"
    end
  end
  mines_left == 0
end

def check_input (x, y)
  x, y = x.to_i - 1, y.to_i - 1
  [x, y] if (0...WIDTH).cover?(x) and (0...HEIGHT).cover?(y)
end

$mines = nil
$screen = Array.new(WIDTH) { Array.new(HEIGHT, ".") }

def play(name) 
  # comenzar a medir el tiempo
  t1 = Time.now
  show_grid
  flag = true
  loop do
    # ingreso de las coordenadas de las celdas
    print "> "
    action = gets.chomp.downcase
    
    case action
    # salir del juego
    when "salir", "x", "q"
      puts "¡Adios!"
      flag = false
      break
    when /^m (\d+) (\d+)$/
      # marcar la celda donde se cree que hay mina
      x, y = check_input($1, $2)
      next unless x
      if $screen[x][y] == "."
        # marcar
        $screen[x][y] = "?"
        # evaluar si se marcaron todas las minas
        if victory?
          show_grid
          puts "¡Ganaste!"
          break
        end
      elsif $screen[x][y] == "?"
        # desmarcar
        $screen[x][y] = "."
      end
      show_grid
    # marcar la celda
    when /^c (\d+) (\d+)$/
      x, y = check_input($1, $2)
      next unless x
      $mines ||= create_mines(x, y)
      # si se pisó una mina
      if $mines[x][y]
        puts "¡Ups! Pisaste una mina... ¡PERDISTE!"
        show_grid true
        flag = false
        break
      else
        clear_space(x, y)
        show_grid
        # evaluar si se marcaron todas las minas correctamente
        if victory?
          puts "¡Ganaste!"
          break
        end
      end
    # mostrar el tablero del juego
    when "p"
      show_grid
    end
  end
  # terminar de tomar el tiempo de la partida
  t2 = Time.now
  # cálculo del tiempo del jugador
  msecs = time_diff_milli t1, t2

  # si el jugador ganó
  if flag == true
    # concatenar nueva línea con tiempo y nombre
    newline = msecs.to_s+","+name.to_s+"\n"
    File.open("tiempos.txt", 'a') do |file|
      # agregar la línea al final del .txt
        file.write newline
    end
  end
end


puts "------------------------------BUSCAMINAS-----------------------------------\n"

puts "Ingrese nombre del jugador: "
name = gets.chomp
name = name.upcase
play(name)

# LEER EL ARCHIVO DE TIEMPOS DE JUGADORES
content = File.read("tiempos.txt") # lee el archivo
lines = content.split("\n") # divide el contenido en líneas
times = [0]
names = [0]
contador = 0
lines.each do |line|
  # cada linea se divide con ",", separo en un array "words"
    words = line.split(",")
    times[contador] = words[0].to_f # casteo a float
    names[contador] = words[1].to_s # casteo a string
    contador = contador + 1
end

# ordenar los tiempos y nombres
bubble_sort(times,names)

puts "\n-------------------------------RESULTADOS-----------------------------------\n"
puts "\nLISTA DE TODOS LOS JUGADORES ORDENADA\n\n"
times.each_with_index do |element,i|
  puts "#{names[i]} con tiempo #{times[i]}"
end

puts "\n---------------------------------TOP 3--------------------------------------\n\n"
# mostrar solo en top 3 de los mejores tiempos 
top = 1
for i in (0..2)
  puts "TOP #{top}: #{names[i]} con tiempo #{times[i]} segundos\n"
  top = top+1
end
