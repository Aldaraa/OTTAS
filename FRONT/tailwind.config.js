/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'success': '#00b96b',
        'primary': '#e57200',
        'primary2': '#00AAAD',
        'secondary': '#9e9e9e',
        'secondary2': '#8592a6',
      },
      borderRadius: {
        'ot': '10px',
      },
      boxShadow: {
        'btn': '0 2px 0 rgba(0, 0, 0, 0.02)',
        'card': 'rgba(50, 50, 93, 0.25) 0px 6px 12px -2px, rgba(0, 0, 0, 0.3) 0px 3px 7px -3px',
        'mini-overlay': 'rgba(0, 0, 0, 0.05) 0px 6px 24px 0px, rgba(0, 0, 0, 0.08) 0px 0px 0px 1px',
        'option': 'rgba(99, 99, 99, 0.2) 0px 2px 8px 0px',
        'option1': 'rgba(0, 0, 0, 0.35) 0px 5px 15px',
        'option2': 'rgba(0, 0, 0, 0.1) 0 4px 30px ',
      },
      animation: {  
        skeleton: 'skeleton 2s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        jump: 'jump .7s ease-in-out',
        scale: 'scaling 3s cubic-bezier(0.4, 0, 0.6, 1) infinite',
      },
      gridTemplateRows: {
        '7': 'repeat(7, minmax(0, 1fr))',
        '8': 'repeat(8, minmax(0, 1fr))',
        '9': 'repeat(9, minmax(0, 1fr))',
        '10': 'repeat(10, minmax(0, 1fr))',
        '15': 'repeat(15, minmax(0, 1fr))',
      },
      keyframes: {
        skeleton: {
          '0%, 100%': { background: '#ddd' },
          '50%': { background: '#eee' },
        },
        shimmer: {
          '100%': {
            transform: 'translateX(100%)',
          }
        },
        jump: {
          '0%': { transform: 'scale(1)'},
          '50%': { transform: 'scale(1.03)'},
          '100%': { transform: 'scale(1)'},
        },
        scaling: {
          '0%': { transform: 'scale(1)'},
          '5%': { transform: 'scale(1.03)'},
          '10%': { transform: 'scale(1)'},
          '100%': { transform: 'scale(1)'},
        }
      },
    },
  },
  plugins: [],
}

