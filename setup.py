from setuptools import setup, find_packages
import sys, os.path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), 'ule'))

setup(name='ule',
      version=0.1,
      description='Unity Learning Environment: A framework for developing and training Reinforcement Learning environments in Unity3D.',
      url='https://github.com/badgeir/ule',
      author='Peter Leupi',
      author_email='pleupi123@gmail.com',
      license='MIT',
      packages=[package for package in find_packages()
                if package.startswith('ule')],
)
