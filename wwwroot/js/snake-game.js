// Modern Snake Game Implementation
const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const startButton = document.getElementById('startButton');
const playerNameInput = document.getElementById('playerName');
const startPlayingButton = document.getElementById('startPlaying');
const usernameSection = document.getElementById('usernameSection');
const gameSection = document.getElementById('gameSection');
const currentPlayerSpan = document.getElementById('currentPlayer');
const scoreSpan = document.getElementById('score');

// Game settings
const gridSize = 20;
const tileCount = 20;
const tileSize = canvas.width / tileCount;

// Modern color scheme from design spec
const colors = {
    background: '#1e1e2e',    // Dark Graphite
    snake: '#4ade80',         // Vibrant Green
    snakeHead: '#22c55e',     // Brighter Neon Green
    food: '#f43f5e',          // Bright Rose Red
    grid: '#2a2a3b',          // Slate Grey
    text: '#f4f4f5',          // Light Grey-White
    gameOverBox: '#0f172a',   // Dark Blue-Gray
    gameOverText: '#f87171'   // Soft Red
};

// Game state
let snake = [
    { x: 10, y: 10 }
];
let food = { x: 15, y: 15 };
let dx = 0;
let dy = 0;
let score = 0;
let gameSpeed = 100;
let gameLoop;
let isGameRunning = false;
let currentPlayer = '';

// Initialize game
function initGame() {
    // Set canvas background
    canvas.style.backgroundColor = colors.background;
    canvas.style.borderRadius = '10px';
    canvas.style.boxShadow = '0 0 20px rgba(0, 0, 0, 0.3)';
    
    // Draw initial state
    drawGrid();
    drawSnake();
    drawFood();
    
    // Start game loop
    gameLoop = setInterval(gameUpdate, gameSpeed);
}

// Draw grid
function drawGrid() {
    ctx.strokeStyle = colors.grid;
    ctx.lineWidth = 0.5;
    
    for (let i = 0; i < tileCount; i++) {
        ctx.beginPath();
        ctx.moveTo(i * tileSize, 0);
        ctx.lineTo(i * tileSize, canvas.height);
        ctx.stroke();
        
        ctx.beginPath();
        ctx.moveTo(0, i * tileSize);
        ctx.lineTo(canvas.width, i * tileSize);
        ctx.stroke();
    }
}

// Draw snake
function drawSnake() {
    snake.forEach((segment, index) => {
        if (index === 0) {
            // Draw snake head with glow effect
            ctx.shadowColor = colors.snakeHead;
            ctx.shadowBlur = 15;
            ctx.fillStyle = colors.snakeHead;
        } else {
            ctx.shadowBlur = 0;
            ctx.fillStyle = colors.snake;
        }
        
        // Add rounded corners
        ctx.beginPath();
        ctx.roundRect(
            segment.x * tileSize,
            segment.y * tileSize,
            tileSize - 1,
            tileSize - 1,
            5
        );
        ctx.fill();
    });
    
    // Reset shadow
    ctx.shadowBlur = 0;
}

// Draw food
function drawFood() {
    ctx.fillStyle = colors.food;
    
    // Draw food with glow effect
    ctx.shadowColor = colors.food;
    ctx.shadowBlur = 15;
    
    ctx.beginPath();
    ctx.arc(
        food.x * tileSize + tileSize/2,
        food.y * tileSize + tileSize/2,
        tileSize/2 - 2,
        0,
        Math.PI * 2
    );
    ctx.fill();
    
    // Reset shadow
    ctx.shadowBlur = 0;
}

// Game update
function gameUpdate() {
    if (!isGameRunning) return;

    // Move snake
    const head = { x: snake[0].x + dx, y: snake[0].y + dy };
    snake.unshift(head);
    
    // Check if snake ate food
    if (head.x === food.x && head.y === food.y) {
        score += 10;
        scoreSpan.textContent = score;
        generateFood();
        // Increase speed slightly
        if (gameSpeed > 50) {
            gameSpeed -= 2;
            clearInterval(gameLoop);
            gameLoop = setInterval(gameUpdate, gameSpeed);
        }
    } else {
        snake.pop();
    }
    
    // Check collision
    if (checkCollision()) {
        gameOver();
        return;
    }
    
    // Clear canvas and redraw
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    drawGrid();
    drawSnake();
    drawFood();
}

// Generate new food position
function generateFood() {
    food = {
        x: Math.floor(Math.random() * tileCount),
        y: Math.floor(Math.random() * tileCount)
    };
    
    // Make sure food doesn't spawn on snake
    while (snake.some(segment => segment.x === food.x && segment.y === food.y)) {
        food = {
            x: Math.floor(Math.random() * tileCount),
            y: Math.floor(Math.random() * tileCount)
        };
    }
}

// Check collision
function checkCollision() {
    const head = snake[0];
    
    // Wall collision
    if (head.x < 0 || head.x >= tileCount || head.y < 0 || head.y >= tileCount) {
        return true;
    }
    
    // Self collision
    for (let i = 1; i < snake.length; i++) {
        if (head.x === snake[i].x && head.y === snake[i].y) {
            return true;
        }
    }
    
    return false;
}

// Game over
function gameOver() {
    clearInterval(gameLoop);
    ctx.fillStyle = colors.gameOverBox;
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    
    ctx.fillStyle = colors.gameOverText;
    ctx.font = '30px Poppins';
    ctx.textAlign = 'center';
    ctx.fillText('Game Over!', canvas.width/2, canvas.height/2 - 20);
    
    ctx.fillStyle = colors.text;
    ctx.font = '20px Roboto Mono';
    ctx.fillText(`Score: ${score}`, canvas.width/2, canvas.height/2 + 20);
    ctx.fillText('Press Space to Restart', canvas.width/2, canvas.height/2 + 50);
}

// Start game
function startGame() {
    if (!isGameRunning) {
        isGameRunning = true;
        startButton.textContent = 'Playing...';
        startButton.disabled = true;
        gameLoop = setInterval(gameUpdate, gameSpeed);
    }
}

// Event listeners
startButton.addEventListener('click', startGame);

startPlayingButton.addEventListener('click', function() {
    currentPlayer = playerNameInput.value.trim();
    if (currentPlayer) {
        currentPlayerSpan.textContent = currentPlayer;
        usernameSection.style.display = 'none';
        gameSection.style.display = 'block';
        initGame();
    } else {
        alert('Please enter your name to play!');
    }
});

// Handle keyboard input
document.addEventListener('keydown', (e) => {
    if (!isGameRunning) return;

    switch(e.key) {
        case 'ArrowUp':
            if (dy !== 1) { dx = 0; dy = -1; }
            break;
        case 'ArrowDown':
            if (dy !== -1) { dx = 0; dy = 1; }
            break;
        case 'ArrowLeft':
            if (dx !== 1) { dx = -1; dy = 0; }
            break;
        case 'ArrowRight':
            if (dx !== -1) { dx = 1; dy = 0; }
            break;
        case ' ':
            if (!gameLoop) {
                // Reset game
                snake = [{ x: 10, y: 10 }];
                dx = 0;
                dy = 0;
                score = 0;
                gameSpeed = 100;
                generateFood();
                initGame();
            }
            break;
    }
});

// Initialize the game when the page loads
initGame();