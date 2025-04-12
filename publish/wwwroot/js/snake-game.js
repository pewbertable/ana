// Snake Game Implementation
document.addEventListener('DOMContentLoaded', function() {
    const canvas = document.getElementById('gameCanvas');
    const ctx = canvas.getContext('2d');
    const startButton = document.getElementById('startButton');
    const playerNameInput = document.getElementById('playerName');
    const startPlayingButton = document.getElementById('startPlaying');
    const usernameSection = document.getElementById('usernameSection');
    const gameSection = document.getElementById('gameSection');
    const currentPlayerSpan = document.getElementById('currentPlayer');
    const scoreSpan = document.getElementById('score');

    // Game variables
    let snake = [];
    let food = {};
    let direction = 'right';
    let gameLoop;
    let score = 0;
    let gameSpeed = 150;
    let isGameRunning = false;
    let currentPlayer = '';

    // Grid size
    const gridSize = 20;
    const tileCount = canvas.width / gridSize;

    // Initialize game
    function initGame() {
        snake = [
            { x: 5, y: 5 }
        ];
        direction = 'right';
        score = 0;
        scoreSpan.textContent = score;
        generateFood();
    }

    // Generate food at random position
    function generateFood() {
        food = {
            x: Math.floor(Math.random() * tileCount),
            y: Math.floor(Math.random() * tileCount)
        };
        // Make sure food doesn't spawn on snake
        for (let segment of snake) {
            if (segment.x === food.x && segment.y === food.y) {
                generateFood();
                break;
            }
        }
    }

    // Game loop
    function gameUpdate() {
        moveSnake();
        if (checkCollision()) {
            gameOver();
            return;
        }
        checkFood();
        draw();
    }

    // Move snake
    function moveSnake() {
        const head = { x: snake[0].x, y: snake[0].y };

        switch (direction) {
            case 'up': head.y--; break;
            case 'down': head.y++; break;
            case 'left': head.x--; break;
            case 'right': head.x++; break;
        }

        snake.unshift(head);
        if (head.x === food.x && head.y === food.y) {
            score += 10;
            scoreSpan.textContent = score;
            generateFood();
        } else {
            snake.pop();
        }
    }

    // Check for collisions
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

    // Check if snake ate food
    function checkFood() {
        const head = snake[0];
        if (head.x === food.x && head.y === food.y) {
            score += 10;
            scoreSpan.textContent = score;
            generateFood();
        }
    }

    // Draw game
    function draw() {
        // Clear canvas
        ctx.fillStyle = '#f8f9fa';
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Draw snake
        ctx.fillStyle = '#28a745';
        for (let segment of snake) {
            ctx.fillRect(segment.x * gridSize, segment.y * gridSize, gridSize - 2, gridSize - 2);
        }

        // Draw food
        ctx.fillStyle = '#dc3545';
        ctx.fillRect(food.x * gridSize, food.y * gridSize, gridSize - 2, gridSize - 2);
    }

    // Game over
    function gameOver() {
        clearInterval(gameLoop);
        isGameRunning = false;
        startButton.textContent = 'Start Game';
        
        // Show game over popup
        showGameOverPopup();
    }

    // Show game over popup
    function showGameOverPopup() {
        const popup = document.createElement('div');
        popup.className = 'game-over-popup';
        popup.innerHTML = `
            <div class="popup-content">
                <h3>Game Over!</h3>
                <p>Your score: ${score}</p>
                <button class="btn btn-primary" id="playAgain">Play Again</button>
            </div>
        `;
        document.body.appendChild(popup);

        // Add event listener for play again button
        document.getElementById('playAgain').addEventListener('click', function() {
            document.body.removeChild(popup);
            initGame();
        });
    }

    // Start game
    function startGame() {
        if (!isGameRunning) {
            isGameRunning = true;
            startButton.textContent = 'Stop Game';
            gameLoop = setInterval(gameUpdate, gameSpeed);
        } else {
            clearInterval(gameLoop);
            isGameRunning = false;
            startButton.textContent = 'Start Game';
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
        }
    });

    // Keyboard controls
    document.addEventListener('keydown', function(e) {
        switch (e.key) {
            case 'ArrowUp':
                if (direction !== 'down') direction = 'up';
                break;
            case 'ArrowDown':
                if (direction !== 'up') direction = 'down';
                break;
            case 'ArrowLeft':
                if (direction !== 'right') direction = 'left';
                break;
            case 'ArrowRight':
                if (direction !== 'left') direction = 'right';
                break;
        }
    });
}); 